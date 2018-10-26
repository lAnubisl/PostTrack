using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.Common;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL
{
    public class PackagePresentationService : IPackagePresentationService
    {
        private const int threadsCount = 4;
        private readonly ILogger _logger;
        private readonly IMessageSender _messageSender;
        private readonly IPackageDAO _packageDAO;
        private readonly IResponseReader _reader;
        private readonly IUpdateSearcher _searcher;
        private readonly Interfaces.IConfigurationService _configurationService;

        public PackagePresentationService(
            IPackageDAO packageDAO,
            IMessageSender messageSender,
            IUpdateSearcher searcher,
            IResponseReader reader,
            Interfaces.IConfigurationService configurationService,
            ILogger logger)
        {
            _packageDAO = packageDAO;
            _messageSender = messageSender;
            _searcher = searcher;
            _reader = reader;
            _logger = logger.CreateScope(nameof(PackagePresentationService));
            _configurationService = configurationService;
        }

        public async Task Register(RegisterTrackingModel model)
        {
            _logger.Info($"Call: {nameof(Register)}(model)");
            var dto = model.Map();
            await _packageDAO.RegisterAsync(dto);
            await SendRegistered(dto);
        }

        public async Task UpdateComingPackages()
        {
            _logger.Info($"Call: {nameof(UpdateComingPackages)}()");
            var packages = await _packageDAO.LoadTrackingAsync();
            if (packages == null)
            {
                _logger.Fatal("PackageDAO returned null");
                return;
            }

            _logger.Info($"Starting update {packages.Count} packages");
            if (packages.Count == 0)
            {
                return;
            }

            await Task.WhenAll(packages.Select(p => UpdatePackage(p)));
        }

        private async Task SendRegistered(RegisterPackageDTO dto)
        {
            _logger.Info($"Call: {nameof(SendRegistered)}(dto)");
            var package = await _packageDAO.LoadAsync(dto.Tracking);
            if (package == null)
            {
                _logger.Fatal($"Cannot find package {dto.Tracking}");
                return;
            }

            var history = await SearchPackageStatus(package);
            await _messageSender.SendRegistered(package, history);

            if (PackageHelper.IsEmpty(history))
            {
                return;
            }

            SavePackageStatus(package, history);
        }

        private async Task<ICollection<PackageHistoryItemDTO>> SearchPackageStatus(PackageDTO package)
        {
            _logger.Info($"Call: {nameof(SearchPackageStatus)}(package)");
            if (string.IsNullOrEmpty(package.Tracking))
            {
                _logger.Fatal("Package has empty tracing. I can't update this package.");
                return null;
            }

            var response = await _searcher.SearchAsync(package);
            if (string.IsNullOrEmpty(response))
            {
                _logger.Error($"Response from web is empty. I can't update package {package.Tracking}");
                return null;
            }

            return _reader.Read(response);
        }

        private async Task UpdatePackage(PackageDTO package)
        {
            _logger.Info($"Call: {nameof(UpdatePackage)}(package)");
            if (PackageHelper.IsFinished(package))
            {
                SavePackageStatus(package, package.History);
                return;
            }

            var history = await SearchPackageStatus(package);
            if (PackageHelper.IsStatusTheSame(history, package))
            {
                if (PackageHelper.IsInactivityPeriodElapsed(package, _configurationService.InactivityPeriodMonths))
                {
                    StopTracking(package);
                }

                _logger.Debug($"No update was found for package {package.Tracking}.");
                return;
            }

            if (history != null)
            {
                _logger.Debug($"Update was Found!!! Sending an update email for package {package.Tracking}.");
                await _messageSender.SendStatusUpdate(package, history);
                SavePackageStatus(package, history);
            }
        }

        private void SavePackageStatus(PackageDTO package, ICollection<PackageHistoryItemDTO> history)
        {
            _logger.Info($"Call: {nameof(SavePackageStatus)}(package, history)");
            package.History = history;
            package.IsFinished = PackageHelper.IsFinished(package);
            _packageDAO.UpdateAsync(package);
        }

        private void StopTracking(PackageDTO package)
        {
            _logger.Warning($"The package {package.Tracking} was inactive for {_configurationService.InactivityPeriodMonths} months. Stop tracking it.");
            _messageSender.SendInactivityEmail(package);
            package.IsFinished = true;
            _packageDAO.UpdateAsync(package);
        }
    }
}