using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.BLL.Models.EmailModels;
using Posttrack.Common;
using Posttrack.Data.Interfaces.DTO;
using SparkPost;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class EmailMessageSender : IMessageSender
    {
        private readonly ISparkPostTemplateProvider _sparkPostTemplateProvider;
        private readonly string _apiKey;
        private readonly ILogger _logger;

        public EmailMessageSender(IConfigurationService settingsProvider, ISparkPostTemplateProvider sparkPostTemplateProvider, ILogger logger)
        {
            _logger = logger.CreateScope(nameof(EmailMessageSender));
            _apiKey = settingsProvider.SparkPostApiKey;
            _sparkPostTemplateProvider = sparkPostTemplateProvider;
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public Task SendStatusUpdateAsync(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            _logger.Info($"Call: {nameof(SendStatusUpdateAsync)}(package, update)");
            return Send(new PackageUpdateEmailModel(package, update), EmailType.PostTrackPackageUpdate);
        }

        public Task SendRegisteredAsync(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            _logger.Info($"Call: {nameof(SendRegisteredAsync)}(package, update)");
            return Send(new PackageRegisteredEmailModel(package, update), EmailType.PostTrackRegistered);
        }

        public Task SendInactivityEmailAsync(PackageDTO package)
        {
            _logger.Info($"Call: {nameof(SendInactivityEmailAsync)}(package)");
            return Send(new PackageTrackingCancelledEmailModel(package), EmailType.PostTrackTrackingCancellation);
        }

        private static Dictionary<string, object> GetSubstitutionData(BaseEmailModel model)
        {
            var dictionary = new Dictionary<string, object>();

            if (model.GetType().BaseType != typeof(BaseEmailModel))
            {
                return dictionary;
            }

            foreach (var field in model.GetType()
                .GetFields(BindingFlags.Instance |
                               BindingFlags.NonPublic |
                               BindingFlags.DeclaredOnly))
            {
                dictionary.Add(field.Name, field.GetValue(model));
            }

            return dictionary;
        }

        private async Task Send(BaseEmailModel model, EmailType emailType)
        {
            _logger.Info($"Call: {nameof(Send)}(model, {emailType})");
            var emailTemplate = await _sparkPostTemplateProvider.GetTemplate(emailType);
            var substitutionData = GetSubstitutionData(model);
            var transmission = new Transmission
            {
                Content =
                    {
                        From = emailTemplate.TemplateContent.From,
                        Text = emailTemplate.TemplateContent.Text,
                        Html = emailTemplate.TemplateContent.Html,
                        Subject = emailTemplate.TemplateContent.Subject,
                    },
                SubstitutionData = substitutionData
            };
            await Transmit(model.Recipient, transmission, emailType);
        }

        private async Task Transmit(string recipientEmail, Transmission transmission, EmailType emailType)
        {
            _logger.Info($"Call: {nameof(Transmit)}({recipientEmail}, transmission, {emailType})");
            try
            {
                transmission.Recipients.Add(new Recipient
                {
                    Address = new Address { Email = recipientEmail }
                });
                var client = new Client(_apiKey);
                await client.Transmissions.Send(transmission);
                _logger.Info($"Message {emailType} sent to {recipientEmail}");
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }
        }
    }
}