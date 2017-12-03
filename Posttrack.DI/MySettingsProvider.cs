using Posttrack.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posttrack.DI
{
    public class MySettingsProvider : ISettingsProvider
    {
        public string SmtpHost => "smtp.gmail.com";

        public string SmtpUser => "reply.posttrack@gmail.com";

        public int SmtpPort => 587;

        public string SmtpPassword => "nothingspecial1!";

        public bool IsSmtpSecured => false;

        public string SmtpFrom => "reply.posttrack@gmail.com";

        public int InactivityPeriodMonths => 2;

        public string HistoryRegex => @"(\d{4}-\d{2}-\d{2}|\d{2}.\d{2}.\d{4}).+?(?&amp;gt;Label1"" & amp; gt;)(.+?(?=&amp;lt;/span&amp;gt;))(.+?(?&amp;gt;size=""4""&amp;gt;)([^ &amp;lt;]+)&amp;lt;/font&amp;gt;&amp;lt;/td&amp;gt;\r\n\t\t\t\t&amp;lt;/tr&amp;gt;)?";

        public string HttpSearchUrl => "http://declaration.belpost.by/searchRu.aspx";
    }
}
