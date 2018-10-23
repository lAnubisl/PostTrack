using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Interfaces.Models;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using Xunit;

namespace Posttrack.BLL.Tests
{
    public class PackagePresentationSetviceTests
    {
        private Mock<IPackageDAO> dao;
        private Mock<IResponseReader> reader;
        private Mock<IUpdateSearcher> searcher;
        private Mock<IMessageSender> sender;
        private Mock<Interfaces.IConfigurationService> configuration;
        private IPackagePresentationService service;

        public PackagePresentationSetviceTests()
        {
            dao = new Mock<IPackageDAO>();
            searcher = new Mock<IUpdateSearcher>();
            reader = new Mock<IResponseReader>();
            sender = new Mock<IMessageSender>();
            configuration = new Mock<Interfaces.IConfigurationService>();
            configuration.Setup(c => c.InactivityPeriodMonths).Returns(2);
            service = new PackagePresentationService(dao.Object, sender.Object, searcher.Object, reader.Object, configuration.Object);
        }

        [Fact]
        public void Register_Shoudl_Call_DAO()
        {
            var model = new RegisterTrackingModel();
            model.Description = "Description";
            model.Email = "email@email.com";
            model.Tracking = "AA123123123PP";
            service.Register(model).Wait();
            dao.Verify(
                c =>
                    c.RegisterAsync(
                        It.Is<RegisterPackageDTO>(
                            d =>
                                d.Tracking == model.Tracking && d.Email == model.Email &&
                                d.Description == model.Description)));
        }

        [Fact]
        public void Register_Shoudl_Call_Sender()
        {
            var model = new RegisterTrackingModel();
            model.Description = "Description";
            model.Email = "email@email.com";
            model.Tracking = "AA123123123PP";

            var savedPackage = new PackageDTO {Tracking = model.Tracking};

            dao.Setup(d => d.LoadAsync(model.Tracking)).Returns(Task.FromResult(savedPackage));
            searcher.Setup(s => s.Search(savedPackage)).Returns("Empty search result");

            service.Register(model).Wait();
            sender.Verify(c => c.SendRegistered(savedPackage, null));
        }

        [Fact]
        public void UpdateComingPachages_Should_Call_Searcher_For_Each_Package()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>();
            packages.Add(new PackageDTO {Tracking = "t1"});
            packages.Add(new PackageDTO {Tracking = "t2"});

            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            service.UpdateComingPackages();
            var arr = packages.ToArray();
            searcher.Verify(c => c.Search(arr[0]));
            searcher.Verify(c => c.Search(arr[1]));
        }

        [Fact]
        public void UpdateComingPachages_Should_Call_Reader_For_Each_Package()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>();
            packages.Add(new PackageDTO {Tracking = "t1"});
            packages.Add(new PackageDTO {Tracking = "t2"});
            packages.Add(new PackageDTO {Tracking = "t3"});

            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Fake result");
            service.UpdateComingPackages();
            reader.Verify(c => c.Read("Fake result"), Times.Exactly(3));
        }

        [Fact]
        public void UpdateComingPackages_Should_Call_SendInactivityEmail_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2)
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {inactivePackage} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages().Wait();

            sender.Verify(c => c.SendInactivityEmail(inactivePackage));
        }

        [Fact]
        public void UpdateComingPackages_Should_Finish_Package_When_Package_Was_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                IsFinished = false,
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2)
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {inactivePackage} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages().Wait();

            dao.Verify(c => c.UpdateAsync(inactivePackage));
            Assert.True(inactivePackage.IsFinished);
        }

        [Fact]
        public void
            UpdateComingPackages_Should_Not_Call_SendInactivityEmail_When_Package_Was_Not_Inactive_For_A_Long_Time()
        {
            var inactivePackage = new PackageDTO
            {
                Tracking = "Not null tracking",
                UpdateDate = DateTime.Now.AddMonths(-2 + 1)
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {inactivePackage} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(null as ICollection<PackageHistoryItemDTO>);

            service.UpdateComingPackages().Wait();

            sender.Verify(c => c.SendInactivityEmail(inactivePackage), Times.Never);
        }

        [Fact]
        public void UpdateComingPackages_Should_Update_Package_History()
        {
            var package = new PackageDTO {Tracking = "Not null tracking", History = null};
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO {Action = "Action", Place = "Place", Date = DateTime.Now}
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {package} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages().Wait();

            dao.Verify(c => c.UpdateAsync(package));
            Assert.Equal(history, package.History);
        }

        [Fact]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_1()
        {
            var package = new PackageDTO {Tracking = "Not null tracking", History = null, IsFinished = false};
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO {Action = "Доставлено, вручено", Place = "Place", Date = DateTime.Now}
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {package} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages();
            Assert.True(package.IsFinished);
        }

        [Fact]
        public void UpdateComingPackages_Should_Finish_Package_When_History_Has_A_Special_Action_2()
        {
            var package = new PackageDTO {Tracking = "Not null tracking", History = null, IsFinished = false};
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO {Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now}
            };
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(new Collection<PackageDTO> {package} as ICollection<PackageDTO>));
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>())).Returns("Not empty search result");
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()));

            service.UpdateComingPackages().Wait();
            Assert.True(package.IsFinished);
        }

        [Fact]
        public void UpdateComingPackages_Should_Work_Async()
        {
            ICollection<PackageDTO> packages = new Collection<PackageDTO>
            {
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"},
                new PackageDTO {Tracking = "Not null tracking"}
            };
            var history = new Collection<PackageHistoryItemDTO>
            {
                new PackageHistoryItemDTO {Action = "Отправление доставлено", Place = "Place", Date = DateTime.Now}
            };
            reader.Setup(r => r.Read(It.IsAny<string>())).Returns(history);
            searcher.Setup(s => s.Search(It.IsAny<PackageDTO>()))
                .Callback(() => Thread.Sleep(500))
                .Returns("Not empty search result");
            sender.Setup(s => s.SendStatusUpdate(It.IsAny<PackageDTO>(), It.IsAny<IEnumerable<PackageHistoryItemDTO>>()))
                .Callback(() => Thread.Sleep(500));
            dao.Setup(d => d.LoadTrackingAsync()).Returns(Task.FromResult(packages));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            service.UpdateComingPackages().Wait();
            stopwatch.Stop();
            Assert.True(stopwatch.Elapsed.Seconds > 1);
            Assert.True(stopwatch.Elapsed.Seconds < 10);
        }
    }
}