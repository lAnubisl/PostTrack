using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Properties;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class EmailMessageSender : IMessageSender
    {
        private readonly SmtpClient smtpClient;
        private readonly IEmailTemplateManager templateManager;

        public EmailMessageSender(IEmailTemplateManager templateManager)
        {
            this.templateManager = templateManager;
            smtpClient = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(Settings.Default.SmtpUser, Settings.Default.SmtpPassword);
            smtpClient.EnableSsl = Settings.Default.SmtpSecure;
            smtpClient.Timeout = 20000;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        void IMessageSender.SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            using (var message = new MailMessage(Settings.Default.SmtpFrom, package.Email))
            {
                message.Subject = GetStatusUpdateEmailSubject(package);
                message.Body = templateManager.GetUpdateStatusEmailBody(package, update);
                message.IsBodyHtml = true;
                smtpClient.Send(message);
            }
        }

        void IMessageSender.SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            using (var message = new MailMessage(Settings.Default.SmtpFrom, package.Email))
            {
                message.Subject = GetRegisteredEmailSubject(package);
                message.Body = templateManager.GetRegisteredEmailBody(package, update);
                message.IsBodyHtml = true;
                smtpClient.Send(message);
            }
        }

        void IMessageSender.SendInactivityEmail(PackageDTO package)
        {
            using (var message = new MailMessage(Settings.Default.SmtpFrom, package.Email))
            {
                message.Subject = GetInactivityEmailSubject(package);
                message.Body = templateManager.GetInactivityEmailBody(package);
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                smtpClient.Send(message);
            }
        }

        private static string GetInactivityEmailSubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.InactivityEmailSubject, package.Description,
                package.Tracking);
        }

        private static string GetStatusUpdateEmailSubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.StatusUpdateEmailSubject, package.Description,
                package.Tracking);
        }

        private static string GetRegisteredEmailSubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.RegisteredEmailSubject, package.Description,
                package.Tracking);
        }
    }
}