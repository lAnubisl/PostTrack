using Posttrack.BLL.Interfaces;
using Posttrack.Web.Properties;

namespace Posttrack.Web
{
    public class SettingsProvider : ISettingsProvider
    {
        string ISettingsProvider.SmtpHost => Settings.Default.SmtpHost;
        string ISettingsProvider.SmtpUser => Settings.Default.SmtpUser;
        int ISettingsProvider.SmtpPort => Settings.Default.SmtpPort;
        string ISettingsProvider.SmtpPassword => Settings.Default.SmtpPassword;
        bool ISettingsProvider.IsSmtpSecured => Settings.Default.IsSmtpSecured;
        string ISettingsProvider.SmtpFrom => Settings.Default.SmtpFrom;
        int ISettingsProvider.InactivityPeriodMonths => Settings.Default.InactivityPeriodMonths;
        string ISettingsProvider.HistoryRegex => Settings.Default.HistoryRegex;
        string ISettingsProvider.HttpSearchUrl => Settings.Default.HttpSearchUrl;
    }
}