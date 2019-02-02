using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.Common;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Tests
{
    [TestFixture]
    public class PackagePresentationSetviceTests
    {
        private Mock<IPackageDAO> _dao;
        private Mock<IResponseReader> _reader;
        private Mock<IUpdateSearcher> _searcher;
        private Mock<IMessageSender> _sender;
        private Mock<Interfaces.IConfigurationService> _configuration;
        private IPackagePresentationService _service;

        [SetUp]
        public void Setup()
        {
            _dao = new Mock<IPackageDAO>();
            _searcher = new Mock<IUpdateSearcher>();
            _reader = new Mock<IResponseReader>();
            _sender = new Mock<IMessageSender>();
            _sender.Setup(s => s.SendStatusUpdateAsync(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>())).Returns(Task.FromResult(0));
            _configuration = new Mock<Interfaces.IConfigurationService>();
            _configuration.Setup(c => c.InactivityPeriodMonths).Returns(2);
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
            _service = new PackagePresentationService(_dao.Object, _sender.Object, _searcher.Object, _reader.Object, _configuration.Object, logger.Object);
        }

        [Test]
        public void Register_Shoudl_Call_DAO()
        {
            var model = new RegisterTrackingModel
            {
                Description = "Description",
                Email = "email@email.com",
                Tracking = "AA123123123PP"
            };
            _service.Register(model).Wait();
            _dao.Verify(
                c =>
                    c.RegisterAsync(
                        It.Is<RegisterPackageDTO>(
                            d =>
                                d.Tracking == model.Tracking && d.Email == model.Email &&
                                d.Description == model.Description)));
        }

        [Test]
        public void Register_Should_Call_Sender()
        {
            var model = new RegisterTrackingModel
            {
                Description = "Description",
                Email = "email@email.com",
                Tracking = "AA123123123PP"
            };

            var savedPackage = new PackageDTO { Tracking = model.Tracking };

            _dao.Setup(d => d.LoadAsync(model.Tracking)).Returns(Task.FromResult(savedPackage));
            _searcher.Setup(s => s.SearchAsync(savedPackage)).Returns(Task.FromResult("Empty search result"));

            _service.Register(model).Wait();
            _sender.Verify(c => c.SendRegisteredAsync(savedPackage, null));
        }

        [Test]
        public void UpdateComingPachages_Should_Call_Searcher_For_Each_Package()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>
            {
                new PackageDTO { Tracking = "t1" },
                new PackageDTO { Tracking = "t2" }
            };

            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            _service.UpdateComingPackages();
            var arr = packages.ToArray();
            _searcher.Verify(c => c.SearchAsync(arr[0]));
            _searcher.Verify(c => c.SearchAsync(arr[1]));
        }

        [Test]
        public void UpdateComingPachages_Should_Call_Reader_For_Each_Package()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>
            {
                new PackageDTO { Tracking = "t1" },
                new PackageDTO { Tracking = "t2" },
                new PackageDTO { Tracking = "t3" }
            };

            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Fake result"));
            _service.UpdateComingPackages();
            _reader.Verify(c => c.Read("Fake result"), Times.Exactly(3));
        }

        [Test]
        public void UpdateComingPackages_Should_Call_SendInactivityEmail_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2)
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { inactivePackage } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);
            _service.UpdateComingPackages().Wait();
            _sender.Verify(c => c.SendInactivityEmailAsync(inactivePackage));
        }

        [Test]
        public void UpdateComingPackages_Should_Finish_Package_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                IsFinished = false,
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2)
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { inactivePackage } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);
            _service.UpdateComingPackages().Wait();
            _dao.Verify(c => c.UpdateAsync(inactivePackage));
            Assert.True(inactivePackage.IsFinished);
        }

        [Test]
        public void
            UpdateComingPackages_Should_Not_Call_SendInactivityEmail_When_Package_Was_Not_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2 + 1)
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { inactivePackage } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);
            _service.UpdateComingPackages().Wait();
            _sender.Verify(c => c.SendInactivityEmailAsync(inactivePackage), Times.Never);
        }

        [Test]
        public void UpdateComingPackages_Should_Update_Package_History()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null };
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO { Action = "Action", Place = "Place", Date = DateTime.Now }
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { package } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            _service.UpdateComingPackages().Wait();
            _dao.Verify(c => c.UpdateAsync(package));
            Assert.AreEqual(history, package.History);
        }

        [Test]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_1()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null, IsFinished = false };
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO { Action = "Доставлено, вручено", Place = "Place", Date = DateTime.Now }
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { package } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            _service.UpdateComingPackages().Wait();
            Assert.True(package.IsFinished);
        }

        [Test]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_2()
        {
            var package = new PackageDTO { Tracking = "Not null tracking", History = null, IsFinished = false };
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO { Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now }
            };
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> { package } as ICollection<PackageDTO>));
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>())).Returns(Task.FromResult("Not empty search result"));
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            _service.UpdateComingPackages().Wait();
            Assert.True(package.IsFinished);
        }

        [Test]
        [Ignore("")]
        public void UpdateComingPackages_Should_Work_Async()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>
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
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO { Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now }
            };
            _reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            _searcher.Setup(s => s.SearchAsync(It.IsAny<PackageDTO>()))
                .Callback(() => Thread.Sleep(500))
                .Returns(Task.FromResult("Not empty search result"));
            _sender.Setup(s => s.SendStatusUpdateAsync(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()))
                .Callback(() => Thread.Sleep(500))
                .Returns(Task.FromResult(0));
            _dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _service.UpdateComingPackages().Wait();
            stopwatch.Stop();
            Assert.True(stopwatch.Elapsed.Seconds > 1);
            Assert.True(stopwatch.Elapsed.Seconds < 10);
        }
    }
}