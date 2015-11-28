using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Tests.Properties;

namespace Posttrack.BLL.Tests
{
    [TestClass]
    public class ResponseReaderTests
    {
        [TestMethod]
        public void Read_Should_Read_Items()
        {
	        var fullResult = Settings.Default.ResponseSample;

            IResponseReader reader = new ResponseReader();
            var history = reader.Read(fullResult).GetEnumerator();
            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 22, 27, 00), history.Current.Date);
            Assert.AreEqual("Вручено", history.Current.Action);
			Assert.AreEqual("MINSK - 34", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 19, 27, 00), history.Current.Date);
            Assert.AreEqual("10. Доставлено, вручено (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 13, 05, 00), history.Current.Date);
            Assert.AreEqual("Попытка вручения", history.Current.Action);
            Assert.AreEqual("MINSK - 34", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 10, 05, 00), history.Current.Date);
            Assert.AreEqual("09. Попытка доставки (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 09, 24, 00), history.Current.Date);
            Assert.AreEqual("06. Поступило в участок обработки почты (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 27, 06, 32, 17), history.Current.Date);
            Assert.AreEqual("08. Передано из (<a href=\"http://zip.belpost.by/zip_code/200400\">200400</a>)  в (<a href=\"http://zip.belpost.by/zip_code/220034\">220034</a>) Минск - 34", history.Current.Action);
			Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 24, 20, 11, 44), history.Current.Date);
            Assert.AreEqual("Прохождение по таможенной зоне (см. <a href=\"http://declaration.belpost.by/search.aspx?search=RF109959764CN\">http://declaration.belpost.by</a>)", history.Current.Action);
			Assert.AreEqual("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 24, 15, 35, 11), history.Current.Date);
            Assert.AreEqual("Поступило в участок обработки почты", history.Current.Action);
            Assert.AreEqual("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2015, 11, 07, 14, 40, 00), history.Current.Date);
            Assert.AreEqual("Получение отправления от отправителя", history.Current.Action);
            Assert.AreEqual("518103*", history.Current.Place);
        }
    }
}