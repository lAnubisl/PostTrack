using System;

namespace Posttrack.BLL.Interfaces
{
    public interface ISettingsService
    {
        string SparkPostApiKey { get; }

        int InactivityPeriodMonths { get; }

        string HistoryRegex { get; }

        Uri HttpSearchUrl { get; }
    }
}