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
        private readonly IEmailTemplateManager templateManager;

        public EmailMessageSender(IEmailTemplateManager templateManager)
        {
            this.templateManager = templateManager;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        void IMessageSender.SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            SendEmail(
                package.Email, 
                StatusUpdatedSubject(package), 
                templateManager.GetUpdateStatusEmailBody(package, update));
        }

        void IMessageSender.SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            SendEmail(
                package.Email,
                RegisteredSubject(package),
                templateManager.GetRegisteredEmailBody(package, update));
        }

        void IMessageSender.SendInactivityEmail(PackageDTO package)
        {
            SendEmail(
                package.Email,
                InactivitySubject(package),
                templateManager.GetInactivityEmailBody(package));
        }

        private static void SendEmail(string toEmail, string subject, string body)
        {
            using (var clinet = GetClient())
            using (var message = new MailMessage(Settings.Default.SmtpFrom, toEmail))
            {
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                clinet.Send(message);
            }
        }

        private static string InactivitySubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.InactivityEmailSubject, package.Description,
                package.Tracking);
        }

        private static string StatusUpdatedSubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.StatusUpdateEmailSubject, package.Description,
                package.Tracking);
        }

        private static string RegisteredSubject(PackageDTO package)
        {
            return string.Format(CultureInfo.CurrentCulture, EmailMessages.RegisteredEmailSubject, package.Description,
                package.Tracking);
        }

        private static SmtpClient GetClient()
        {
            var client = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(Settings.Default.SmtpUser, Settings.Default.SmtpPassword);
            client.EnableSsl = Settings.Default.SmtpSecure;
            client.Timeout = 20000;       
            return client;
        }
    }
}