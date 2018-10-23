using Moq;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using Xunit;

namespace Posttrack.BLL.Tests
{
	public class BelpostSearcherTests
	{
        [Fact]
        public void Search_Should_Find()
		{
            var settingDao = new Mock<ISettingDAO>();
            settingDao.Setup(s => s.Get(It.IsAny<string>())).Returns(string.Empty);
            IUpdateSearcher searcher = new BelpostSearcher(new ConfigurationService(settingDao.Object));
			var result = searcher.Search(new PackageDTO {Tracking = "RM611628067CN" });
            Assert.NotNull(result);
			Assert.Contains("Доставлено, вручено", result);
		}
	}
}