using System;
using Moq;
using NUnit.Framework;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Common;
using Posttrack.Data.Interfaces;

namespace Posttrack.BLL.Tests
{
    [TestFixture]
    public class ResponseReaderTests
    {
        [Test]
        public void Read_Should_Read_Items()
        {
            var settingDao = new Mock<ISettingDAO>();
            var logger = new Mock<ILogger>();
            logger.Setup(l => l.CreateScope(It.IsAny<string>())).Returns(logger.Object);
            settingDao.Setup(s => s.Load(It.IsAny<string>())).Returns(string.Empty);
            IResponseReader reader = new ResponseReader(new ConfigurationService(settingDao.Object), logger.Object);
            var history = reader.Read(Samples.Sample).GetEnumerator();
            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 18), history.Current.Date);
            Assert.AreEqual("Вручено", history.Current.Action);
            Assert.AreEqual("MINSK - 34", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 18), history.Current.Date);
            Assert.AreEqual("10. Доставлено, вручено (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.AreEqual("09. Попытка доставки (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.AreEqual("08. Передано из (<a href=\"http://zip.belpost.by/zip_code/200400\">200400</a>)  в (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 16), history.Current.Date);
            Assert.AreEqual("06. Поступило в участок обработки почты (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 12), history.Current.Date);
            Assert.AreEqual("Прохождение по таможенной зоне (см. <a href=\"http://declaration.belpost.by/search.aspx?search=RA133251313FI\">http://declaration.belpost.by</a>)", history.Current.Action);
            Assert.AreEqual("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 10), history.Current.Date);
            Assert.AreEqual("Поступило в участок обработки почты", history.Current.Action);
            Assert.AreEqual("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2016, 01, 05), history.Current.Date);
            Assert.AreEqual("Отправка отправления из учреждения обмена", history.Current.Action);
            Assert.AreEqual("HELSINKI", history.Current.Place);
        }
    }
}