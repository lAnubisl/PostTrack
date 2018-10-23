﻿using Posttrack.Data.Interfaces;

namespace Posttrack.BLL
{
    public class ConfigurationService : Interfaces.IConfigurationService
    {
        public ConfigurationService(ISettingDAO settingDAO)
        {
            SparkPostApiKey = settingDAO.Get("SparkPostApiKey");
        }

        public string SparkPostApiKey { get; }

        public int InactivityPeriodMonths => 2;

        public string HistoryRegex => @"(\d{4}-\d{2}-\d{2}|\d{2}.\d{2}.\d{4}).+?(?>Label1"" >)(.+?(?=</span>))(.+?(?>size=""4"">)([^<]+)</font></td>\r\n\t\t\t\t</tr>)?";

        public string HttpSearchUrl => "http://declaration.belpost.by/searchRu.aspx";
    }
}