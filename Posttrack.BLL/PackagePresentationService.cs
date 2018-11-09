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

            _logger.Warning($"Starting update {packages.Count} packages");
            if (packages.Count == 0)
            {
                return;
            }

            var tasks = packages.Select(p => UpdatePackage(p)).ToArray();
            await Task.WhenAll(tasks);
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
            await _messageSender.SendRegisteredAsync(package, history);

            if (PackageHelper.IsEmpty(history))
            {
                return;
            }

            await SavePackageStatusAsync(package, history);
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
                await SavePackageStatusAsync(package, package.History);
                return;
            }

            var history = await SearchPackageStatus(package);
            if (PackageHelper.IsStatusTheSame(history, package))
            {
                if (PackageHelper.IsInactivityPeriodElapsed(package, _configurationService.InactivityPeriodMonths))
                {
                    await StopTrackingAsync(package);
                }

                _logger.Warning($"No update was found for package {package.Tracking}.");
                return;
            }

            if (history != null)
            {
                _logger.Warning($"Update was Found!!! Sending an update email for package {package.Tracking}.");
                await _messageSender.SendStatusUpdateAsync(package, history);
                await SavePackageStatusAsync(package, history);
            }
        }

        private Task SavePackageStatusAsync(PackageDTO package, ICollection<PackageHistoryItemDTO> history)
        {
            _logger.Info($"Call: {nameof(SavePackageStatusAsync)}(package, history)");
            package.History = history;
            package.IsFinished = PackageHelper.IsFinished(package);
            return _packageDAO.UpdateAsync(package);
        }

        private Task StopTrackingAsync(PackageDTO package)
        {
            _logger.Warning($"The package {package.Tracking} was inactive for {_configurationService.InactivityPeriodMonths} months. Stop tracking it.");
            _messageSender.SendInactivityEmailAsync(package);
            package.IsFinished = true;
            return _packageDAO.UpdateAsync(package);
        }
    }
}