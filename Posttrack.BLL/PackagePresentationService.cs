using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL
{
    public class PackagePresentationService : IPackagePresentationService
    {
        private const int threadsCount = 4;
        //private static readonly ILog log = LogManager.GetLogger(typeof (PackagePresentationService));
        private readonly IMessageSender messageSender;
        private readonly IPackageDAO packageDAO;
        private readonly IResponseReader reader;
        private readonly IUpdateSearcher searcher;
        private readonly Interfaces.IConfigurationService settings;
        private TaskScheduler taskScheduler;

        public PackagePresentationService(
            IPackageDAO packageDAO,
            IMessageSender messageSender,
            IUpdateSearcher searcher,
            IResponseReader reader,
            Interfaces.IConfigurationService settings)
        {
            this.packageDAO = packageDAO;
            this.messageSender = messageSender;
            this.searcher = searcher;
            this.reader = reader;
            this.settings = settings;
        }

        async Task IPackagePresentationService.Register(RegisterTrackingModel model)
        {
            var dto = model.Map();
            //log.InfoFormat("Registration {0}", dto.Tracking);
            await packageDAO.RegisterAsync(dto);
            await SendRegistered(dto);
        }

        async Task IPackagePresentationService.UpdateComingPackages()
        {
            var packages = await packageDAO.LoadTrackingAsync();
            if (packages == null)
            {
                //log.Fatal("PackageDAO returned null");
                return;
            }

            //log.InfoFormat("Starting update {0} packages", packages.Count);
            if (packages.Count == 0)
            {
                return;
            }

            var task = new Task(() =>
            {
                var options = new ParallelOptions {MaxDegreeOfParallelism = threadsCount};
                Parallel.ForEach(packages, options, p =>
                {
                    try
                    {
                        UpdatePackage(p);
                    }
                    catch (Exception e)
                    {
                        //log.Fatal(e.Message + " " + e.StackTrace);
                    }
                });
            });
            task.RunSynchronously();
        }

        private async Task SendRegistered(RegisterPackageDTO dto)
        {
            var package = await packageDAO.LoadAsync(dto.Tracking);
            if (package == null)
            {
                //log.FatalFormat("Cannot find package {0}", dto.Tracking);
                return;
            }

            var history = SearchPackageStatus(package);

            await messageSender.SendRegistered(package, history);

            if (PackageHelper.IsEmpty(history))
            {
                return;
            }

            SavePackageStatus(package, history);
        }

        private ICollection<PackageHistoryItemDTO> SearchPackageStatus(PackageDTO package)
        {
            //log.WarnFormat("Starting search package {0} in thread {1}", package.Tracking,
                //Thread.CurrentThread.ManagedThreadId);

            if (string.IsNullOrEmpty(package.Tracking))
            {
                //log.Fatal("Package has empty tracing. I can't update this package.");
                return null;
            }

            var response = searcher.Search(package);
            if (string.IsNullOrEmpty(response))
            {
                //log.ErrorFormat("Response from web is empty. I can't update package {0}", package.Tracking);
                return null;
            }

            return reader.Read(response);
        }

        private void UpdatePackage(PackageDTO package)
        {
            // In case package is already finished then just finish it.
            if (PackageHelper.IsFinished(package))
            {
                SavePackageStatus(package, package.History);
                return;
            }

            var history = SearchPackageStatus(package);
            if (PackageHelper.IsStatusTheSame(history, package))
            {
                if (PackageHelper.IsInactivityPeriodElapsed(package, settings.InactivityPeriodMonths))
                {
                    StopTracking(package);
                }

                //log.DebugFormat("No update was found for package {0}", package.Tracking);
                return;
            }

            if (history != null)
            {
                //log.DebugFormat("Update was Found!!! Sending an update email for package {0}", package.Tracking);
                messageSender.SendStatusUpdate(package, history);
                SavePackageStatus(package, history);
            }
        }

        private void SavePackageStatus(PackageDTO package, ICollection<PackageHistoryItemDTO> history)
        {
            package.History = history;
            package.IsFinished = PackageHelper.IsFinished(package);

            //log.WarnFormat("Updating status for package {0}", package.Tracking);
            packageDAO.UpdateAsync(package);
        }

        private void StopTracking(PackageDTO package)
        {
            //log.WarnFormat("The package {0} was inactive for {1} months. Stop tracking it.", 
                //package.Tracking,
                //settings.InactivityPeriodMonths);
            messageSender.SendInactivityEmail(package);
            package.IsFinished = true;
            packageDAO.UpdateAsync(package);
        }
    }
}