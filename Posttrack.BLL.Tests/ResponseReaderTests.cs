using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using Posttrack.BLL.Helpers.Implementations;
using Posttrack.Data.Interfaces.DTO;
using Posttrack.BLL.Helpers.Interfaces;

namespace Posttrack.BLL.Tests
{
    [TestClass]
    public class ResponseReaderTests
    {
        [TestMethod]
        public void Read_Should_Read_Items()
        {
            var fullResult = @"<table width=""100%"" class=""tbl"">
<tr>
<td class=""theader"">Дата</td>
<td class=""theader"">Событие</td>
<td class=""theader"">POST OFFICE</td>
<!--<td class=""theader"">Код почтового офиса приема</td>-->
<!--<td class=""theader"">Код почтового офиса отправления</td>-->
<!--<td class=""theader"">Ф.И.О. оператора</td>-->
<!--<td class=""theader"">Комментарии</td>-->
</tr>
    <tr>
    <td>2014-06-16 13:11:00</td>
    <td>Получение отправления от отправителя</td>
    <td>MD2024*</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
    <tr>
    <td>2014-06-16 19:39:00</td>
    <td>Получение отправления в учреждение обмена</td>
    <td>CHISINAU CPTP</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
    <tr>
    <td>2014-06-16 20:46:00</td>
    <td>Отправка отправления из учреждения обмена</td>
    <td>CHISINAU CPTP</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
    <tr>
    <td>2014-06-27 12:27:34</td>
    <td>Получение отправления в учреждение обмена</td>
    <td>MINSK PI 2</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
    <tr>
    <td>2014-06-28 04:42:41</td>
    <td>Отправка отправления в местное учреждение</td>
    <td>MINSK PI 3</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
    <tr>
    <td>2014-06-30 21:19:00</td>
    <td>Отправление доставлено</td>
    <td>MINSK - 34</td>
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td width=""100px""><a href=""http://zip.belpost.by/zip_code/"" ></a></td>-->
     <!-- <td></td>-->
     <!-- <td></td>-->
    </tr>
</table>

Прохождение внутри страны
<table width=""100%"" class=""tbl"">
<tr>
<td class=""theader"">Дата</td>
<td class=""theader"">Событие</td>
</tr>
    <tr>
    <td>28.06.2014 04:42:41</td>
    <td>08. Передано из (<a href=""http://zip.belpost.by/zip_code/200400"">200400</a>) Минск ПОПП в (<a href=""http://zip.belpost.by/zip_code/220034"">220034</a>) Минск - 34</td>
    </tr>
    <tr>
    <td>28.06.2014 05:31:00</td>
    <td>06. Поступило в участок обработки почты (<a href=""http://zip.belpost.by/zip_code/220034"">220034</a>) Минск - 34</td>
    </tr>
    <tr>
    <td>30.06.2014 18:19:00</td>
    <td>10. Доставлено, вручено (<a href=""http://zip.belpost.by/zip_code/220034"">220034</a>) Минск - 34</td>
    </tr>
</table>

".Replace("\r", string.Empty);

            IResponseReader reader = new ResponseReader();
            var history = reader.Read(fullResult).GetEnumerator();
            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 30, 21, 19, 00), history.Current.Date);
            Assert.AreEqual("Отправление доставлено", history.Current.Action);
            Assert.AreEqual("MINSK - 34", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 30, 18, 19, 00), history.Current.Date);
            Assert.AreEqual("10. Доставлено, вручено (220034) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 28, 05, 31, 00), history.Current.Date);
            Assert.AreEqual("06. Поступило в участок обработки почты (220034) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 28, 04, 42, 41), history.Current.Date);
            Assert.AreEqual("Отправка отправления в местное учреждение", history.Current.Action);
            Assert.AreEqual("MINSK PI 3", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 28, 04, 42, 41), history.Current.Date);
            Assert.AreEqual("08. Передано из (200400) Минск ПОПП в (220034) Минск - 34", history.Current.Action);
            Assert.AreEqual(string.Empty, history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 27, 12, 27, 34), history.Current.Date);
            Assert.AreEqual("Получение отправления в учреждение обмена", history.Current.Action);
            Assert.AreEqual("MINSK PI 2", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 16, 20, 46, 00), history.Current.Date);
            Assert.AreEqual("Отправка отправления из учреждения обмена", history.Current.Action);
            Assert.AreEqual("CHISINAU CPTP", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 16, 19, 39, 00), history.Current.Date);
            Assert.AreEqual("Получение отправления в учреждение обмена", history.Current.Action);
            Assert.AreEqual("CHISINAU CPTP", history.Current.Place);

            history.MoveNext();
            Assert.AreEqual(new DateTime(2014, 06, 16, 13, 11, 00), history.Current.Date);
            Assert.AreEqual("Получение отправления от отправителя", history.Current.Action);
            Assert.AreEqual("MD2024*", history.Current.Place);
        }
    }
}