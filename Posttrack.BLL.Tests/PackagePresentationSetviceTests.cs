using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.Data.Interfaces;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.Data.Interfaces.DTO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Posttrack.BLL.Properties;
using System.Threading;
using System.Diagnostics;

namespace Posttrack.BLL.Tests
{
    [TestClass]
    public class PackagePresentationSetviceTests
    {
        private Mock<IPackageDAO> dao;
        private Mock<IUpdateSearcher> searcher;
        private Mock<IResponseReader> reader;
        private Mock<IMessageSender> sender;
        private IPackagePresentationService service;
        private DeterministicTaskScheduler determenisticScheduler;

        [TestInitialize]
        public void Init()
        {
            dao = new Mock<IPackageDAO>();
            searcher = new Mock<IUpdateSearcher>();
            reader = new Mock<IResponseReader>();
            sender = new Mock<IMessageSender>();
            determenisticScheduler = new DeterministicTaskScheduler();
            service = new PackagePresentationService(dao.Object, sender.Object, searcher.Object, reader.Object);     
        }

        [TestMethod]
        public void Register_Shoudl_Call_DAO()
        {
            var model = new RegisterTrackingModel();
            model.Description = "Description";
            model.Email = "email@email.com";
            model.Tracking = "AA123123123PP";
            ((PackagePresentationService)service).TaskScheduler = determenisticScheduler;
            service.Register(model);
            dao.Verify(c => c.Register(It.Is<RegisterPackageDTO>(d => d.Tracking == model.Tracking && d.Email == model.Email && d.Description == model.Description)));
        }

        [TestMethod]
        public void Register_Shoudl_Call_Sender()
        {
            var model = new RegisterTrackingModel();
            model.Description = "Description";
            model.Email = "email@email.com";
            model.Tracking = "AA123123123PP";

            var savedPackage = new PackageDTO() { Tracking = model.Tracking };

            dao.Setup(d => d.Load(model.Tracking)).Returns(savedPackage);
            searcher.Setup(s => s.Search(savedPackage)).Returns("Empty search result");
            ((PackagePresentationService)service).TaskScheduler = determenisticScheduler;

            service.Register(model);
            determenisticScheduler.RunPendingTasks();
            sender.Verify(c => c.SendRegistered(savedPackage, null));
        }

        [TestMethod]
        public void UpdateComingPachages_Should_Call_Searcher_For_Each_Package()
        {
            var packages = new Collection<PackageDTO>();
            packages.Add(new PackageDTO { Tracking = "t1" });
            packages.Add(new PackageDTO { Tracking = "t2" });

            dao.Setup(d => d.LoadComingPackets()).Returns(packages);
            service.UpdateComingPackages();
            searcher.Verify(c => c.Search(packages[0]));
            searcher.Verify(c => c.Search(packages[1]));
        }

        [TestMethod]
        public void UpdateComingPachages_Should_Call_Reader_For_Each_Package()
        {
            var packages = new Collection<PackageDTO>();
            packages.Add(new PackageDTO { Tracking = "t1" });
            packages.Add(new PackageDTO { Tracking = "t2" });
            packages.Add(new PackageDTO { Tracking = "t3" });

            dao.Setup(d => d.LoadComingPackets()).Returns(packages);
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Fake result");
            service.UpdateComingPackages();
            reader.Verify(c => c.Read("Fake result"), Times.Exactly(3));
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Call_SendInactivityEmail_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO { Tracking = "Not null tracking", UpdateDate = DateTime.Now.AddMonths(-Settings.Default.InactivityPeriodInMonths) };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { inactivePackage });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages();

            sender.Verify(c => c.SendInactivityEmail(inactivePackage));
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Finish_Package_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO { IsFinished = false, Tracking = "Not null tracking", UpdateDate = DateTime.Now.AddMonths(-Settings.Default.InactivityPeriodInMonths) };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { inactivePackage });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages();

            dao.Verify(c => c.Update(inactivePackage));
            Assert.IsTrue(inactivePackage.IsFinished);
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Not_Call_SendInactivityEmail_When_Package_Was_Not_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO { Tracking = "Not null tracking", UpdateDate = DateTime.Now.AddMonths(-Settings.Default.InactivityPeriodInMonths + 1) };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { inactivePackage });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages();

            sender.Verify(c => c.SendInactivityEmail(inactivePackage), Times.Never);
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Update_Package_History()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null };
            var history = new Collection<PackageHistoryItemDTO>() { new PackageHistoryItemDTO() { Action = "Action", Place = "Place", Date = DateTime.Now } };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { package });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages();

            dao.Verify(c => c.Update(package));
            Assert.AreEqual(history, package.History);
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_1()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null, IsFinished = false };
            var history = new Collection<PackageHistoryItemDTO>() { new PackageHistoryItemDTO() { Action = "Доставлено, вручено", Place = "Place", Date = DateTime.Now } };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { package });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages();
            Assert.IsTrue(package.IsFinished);
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_2()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null, IsFinished = false };
            var history = new Collection<PackageHistoryItemDTO>() { new PackageHistoryItemDTO() { Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now } };
            dao.Setup(d => d.LoadComingPackets()).Returns(new Collection<PackageDTO>() { package });
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages();
            Assert.IsTrue(package.IsFinished);
        }

        [TestMethod]
        public void UpdateComingPackages_Should_Work_Async()
        {
            var packages = new Collection<PackageDTO>()
            {
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" },
                new PackageDTO { Tracking = "Not null tracking" }
            };
            var history = new Collection<PackageHistoryItemDTO>() { new PackageHistoryItemDTO() { Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now } };
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>()))
                .Callback(() => Thread.Sleep(500))
                .Returns("Not empty search result");
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()))
                .Callback(() => Thread.Sleep(500));
            dao.Setup(d => d.LoadComingPackets()).Returns(packages);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            service.UpdateComingPackages();
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.Elapsed.Seconds > 1);
            Assert.IsTrue(stopwatch.Elapsed.Seconds < 10);
        }
    }
}
