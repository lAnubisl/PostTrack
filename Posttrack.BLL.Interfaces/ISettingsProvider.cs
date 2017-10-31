namespace Posttrack.BLL
{
    public interface ISettingsProvider
    {
        string SmtpHost { get; }
        string SmtpUser { get; }
        int SmtpPort { get; }
        string SmtpPassword { get; }
        bool IsSmtpSecured { get; }
        string SmtpFrom { get; }
        int InactivityPeriodMonths { get; }
        string HistoryRegex { get; }
        string HttpSearchUrl { get; }
    }
}