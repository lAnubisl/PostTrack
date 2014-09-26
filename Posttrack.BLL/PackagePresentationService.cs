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

namespace Posttrack.BLL
{
    public class PackagePresentationService : IPackagePresentationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (PackagePresentationService));
        private readonly IMessageSender messageSender;
        private readonly IPackageDAO packageDAO;
        private readonly IResponseReader reader;
        private readonly IUpdateSearcher searcher;

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

        public void Register(RegisterTrackingModel model)
        {
            RegisterPackageDTO dto = model.Map();
            log.InfoFormat("Registration {0}", dto.Tracking);
            packageDAO.Register(dto);
            messageSender.SendRegistered(dto);
        }

        public void UpdateComingPackages()
        {
            ICollection<PackageDTO> packages = packageDAO.LoadComingPackets();
            if (packages == null)
            {
                log.Fatal("PackageDAO returned null");
                return;
            }

            log.InfoFormat("Starting update {0} packages", packages.Count);
            ThreadPool.SetMaxThreads(packages.Count, packages.Count);
            if (packages.Count == 0)
            {
                return;
            }

            //var tasks = new Collection<Task>();

            Parallel.ForEach(packages, UpdatePackage);

            //foreach (var package in packages)
            //{
            //    log.DebugFormat("Starting update package {0}", package.Tracking);
            //    tasks.Add(Task.Factory.StartNew(() => UpdatePackage(package)));
            //}

            //Task.WaitAll(tasks.ToArray());
        }

        private void UpdatePackage(PackageDTO package)
        {
            log.DebugFormat("Starting update package {0}", package.Tracking);

            if (string.IsNullOrEmpty(package.Tracking))
            {
                log.Fatal("Package has empty tracing. I can't update this package.");
                return;
            }

            var response = searcher.Search(package);
            if (string.IsNullOrEmpty(response))
            {
                log.ErrorFormat("Response from web is empty. I can't update package {0}", package.Tracking);
                return;
            }

            var history = reader.Read(response);
            if ((IsEmpty(history) && IsEmpty(package.History)) || (package.History != null && package.History.Count == history.Count))
            {
                if (package.UpdateDate < DateTime.Now.AddMonths(-Settings.Default.InactivityPeriodInMonths))
                {
                    log.WarnFormat("The package {0} was inactive for {1} months. Stop tracking it.", package.Tracking, Settings.Default.InactivityPeriodInMonths);
                    messageSender.SendInactivityEmail(package);
                    package.IsFinished = true;
                    packageDAO.Update(package);
                }

                log.DebugFormat("No update was found for package {0}", package.Tracking);
                return;
            }

            log.DebugFormat("Update was Found!!! Sending an update email for package {0}", package.Tracking);
            if (messageSender.SendStatusUpdate(package, history))
            {
                package.History = history;
                string lastHistoryAction = package.History.Last().Action;
                package.IsFinished = lastHistoryAction != null &&
                                     (lastHistoryAction.Contains("Доставлено, вручено") ||
                                      lastHistoryAction == "Отправление доставлено");
                log.WarnFormat("Updating status for package {0}", package.Tracking);
                packageDAO.Update(package);
            }
        }

        private static bool IsEmpty(IEnumerable<PackageHistoryItemDTO> history)
        {
            return history == null || !history.Any();
        }
    }
}