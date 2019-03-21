using System;
using Posttrack.Data.Interfaces;

namespace Posttrack.BLL
{
    public class SettingsService : Interfaces.ISettingsService
    {
        public SettingsService(ISettingDAO settingDAO)
        {
            SparkPostApiKey = settingDAO.Load("SparkPostApiKey");
            HttpSearchUrl = new Uri(settingDAO.Load("HttpSearchUrl"));
            HistoryRegex = settingDAO.Load("HistoryRegex");
            InactivityPeriodMonths = int.Parse(settingDAO.Load("InactivityPeriodMonths"));
        }

        public string SparkPostApiKey { get; }

        public int InactivityPeriodMonths { get; }

        public string HistoryRegex { get; }

        public Uri HttpSearchUrl { get; }
    }
}