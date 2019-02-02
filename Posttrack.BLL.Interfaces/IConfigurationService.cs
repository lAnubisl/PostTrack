using System;

namespace Posttrack.BLL.Interfaces
{
    public interface IConfigurationService
    {
        string SparkPostApiKey { get; }

        int InactivityPeriodMonths { get; }

        string HistoryRegex { get; }

        Uri HttpSearchUrl { get; }
    }
}