using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.BLL.Properties;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Concurrent;

namespace Posttrack.BLL
{
    public class PackagePresentationService : IPackagePresentationService
    {
        private static readonly int threadsCount = 4;
        private static readonly ILog log = LogManager.GetLogger(typeof (PackagePresentationService));
        private readonly IMessageSender messageSender;
        private readonly IPackageDAO packageDAO;
        private readonly IResponseReader reader;
        private readonly IUpdateSearcher searcher;
        private TaskScheduler taskScheduler;

        internal TaskScheduler TaskScheduler
        {
            get 
            {
                return taskScheduler ?? new LimitedConcurrencyLevelTaskScheduler(threadsCount); 
            }
            set 
            { 
                taskScheduler = value; 
            }
        }

        public PackagePresentationService(
            IPackageDAO packageDAO, 
            IMessageSender messageSender, 
            IUpdateSearcher searcher,
            IResponseReader reader)
        {
            this.packageDAO = packageDAO;
            this.messageSender = messageSender;
            this.searcher = searcher;
            this.reader = reader;
        }

        void IPackagePresentationService.Register(RegisterTrackingModel model)
        {
            RegisterPackageDTO dto = model.Map();
            log.InfoFormat("Registration {0}", dto.Tracking);
            packageDAO.Register(dto);
            Task.Factory.StartNew(() => SendRegistered(dto), CancellationToken.None, TaskCreationOptions.None, TaskScheduler);
        }

        void IPackagePresentationService.UpdateComingPackages()
        {
            ICollection<PackageDTO> packages = packageDAO.LoadComingPackets();
            if (packages == null)
            {
                log.Fatal("PackageDAO returned null");
                return;
            }

            log.InfoFormat("Starting update {0} packages", packages.Count);       
            if (packages.Count == 0)
            {
                return;
            }

            ThreadPool.SetMaxThreads(threadsCount, threadsCount);
            ThreadPool.SetMinThreads(threadsCount, threadsCount);

            var task = new Task(() => {
                var options = new ParallelOptions { TaskScheduler = this.TaskScheduler, MaxDegreeOfParallelism = threadsCount };
                Parallel.ForEach(packages, options, p =>
                {
                    try
                    {
                        UpdatePackage(p);
                    }
                    catch (Exception e)
                    {
                        log.Fatal(e.Message + " " + e.StackTrace);
                    }
                });
            });
            task.RunSynchronously(TaskScheduler);
        }

        private void SendRegistered(RegisterPackageDTO dto)
        {
            var package = packageDAO.Load(dto.Tracking);
            if (package == null)
            {
                log.FatalFormat("Cannot find package {0}", dto.Tracking);
                return;
            }

            var history = SearchPackageStatus(package);

            messageSender.SendRegistered(package, history);

            if (PackageHelper.IsEmpty(history))
            {
                return;
            }

            SavePackageStatus(package, history);
        }

        private ICollection<PackageHistoryItemDTO> SearchPackageStatus(PackageDTO package)
        {
            log.WarnFormat("Starting search package {0} in thread {1}", package.Tracking, Thread.CurrentThread.ManagedThreadId);

            if (string.IsNullOrEmpty(package.Tracking))
            {
                log.Fatal("Package has empty tracing. I can't update this package.");
                return null;
            }

            var response = searcher.Search(package);
            if (string.IsNullOrEmpty(response))
            {
                log.ErrorFormat("Response from web is empty. I can't update package {0}", package.Tracking);
                return null;
            }

            return reader.Read(response);
        }

        private void UpdatePackage(PackageDTO package)
        {
            var history = SearchPackageStatus(package);
            if (PackageHelper.IsStatusTheSame(history, package))
            {
                if (PackageHelper.IsInactivityPeriodElapsed(package))
                {
                    StopTracking(package);
                }

                log.DebugFormat("No update was found for package {0}", package.Tracking);
                return;
            }

            log.DebugFormat("Update was Found!!! Sending an update email for package {0}", package.Tracking);
            
            messageSender.SendStatusUpdate(package, history);
            SavePackageStatus(package, history);
        }

        private void SavePackageStatus(PackageDTO package, ICollection<PackageHistoryItemDTO> history)
        {
            package.History = history;
            package.IsFinished = PackageHelper.IsFinished(package);

            log.WarnFormat("Updating status for package {0}", package.Tracking);
            packageDAO.Update(package);
        }

        private void StopTracking(PackageDTO package)
        {
            log.WarnFormat("The package {0} was inactive for {1} months. Stop tracking it.", package.Tracking, Settings.Default.InactivityPeriodInMonths);
            messageSender.SendInactivityEmail(package);
            package.IsFinished = true;
            packageDAO.Update(package);
        }
    }
}