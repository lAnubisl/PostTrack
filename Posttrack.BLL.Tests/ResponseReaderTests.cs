using System;
using Moq;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces;
using Xunit;

namespace Posttrack.BLL.Tests
{
    public class ResponseReaderTests
    {
        [Fact]
        public void Read_Should_Read_Items()
        {
            var settingDao = new Mock<ISettingDAO>();
            settingDao.Setup(s => s.Get(It.IsAny<string>())).Returns(string.Empty);
            IResponseReader reader = new ResponseReader(new ConfigurationService(settingDao.Object));
            var history = reader.Read(Samples.Sample).GetEnumerator();
            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 18), history.Current.Date);
            Assert.Equal("Вручено", history.Current.Action);
			Assert.Equal("MINSK - 34", history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 18), history.Current.Date);
            Assert.Equal("10. Доставлено, вручено (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.Equal(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.Equal("09. Попытка доставки (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.Equal(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.Equal("08. Передано из (<a href=\"http://zip.belpost.by/zip_code/200400\">200400</a>)  в (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.Equal(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.Equal("06. Поступило в участок обработки почты (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
            Assert.Equal(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 12), history.Current.Date);
            Assert.Equal("Прохождение по таможенной зоне (см. <a href=\"http://declaration.belpost.by/search.aspx?search=RA133251313FI\">http://declaration.belpost.by</a>)", history.Current.Action);
            Assert.Equal("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 10), history.Current.Date);
            Assert.Equal("Поступило в участок обработки почты", history.Current.Action);
            Assert.Equal("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.Equal(new DateTime(2016, 01, 05), history.Current.Date);
            Assert.Equal("Отправка отправления из учреждения обмена", history.Current.Action);
            Assert.Equal("HELSINKI", history.Current.Place);
        }
    }
}