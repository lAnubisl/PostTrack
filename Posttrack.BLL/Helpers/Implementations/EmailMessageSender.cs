using System.Collections.Generic;
using System.Net;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using Posttrack.BLL.Interfaces;
using System.Threading.Tasks;
using SparkPost;
using System;
using Posttrack.BLL.Models.EmailModels;
using System.Reflection;
using Posttrack.Common;

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
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public Task SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            _logger.Info($"Call: {nameof(SendStatusUpdate)}(package, update)");
            return Send(new PackageUpdateEmailModel(package, update), EmailTypes.PostTrackPackageUpdate);
        }

        public Task SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            _logger.Info($"Call: {nameof(SendRegistered)}(package, update)");
            return Send(new PackageRegisteredEmailModel(package, update), EmailTypes.PostTrackRegistered);
        }

        public Task SendInactivityEmail(PackageDTO package)
        {
            _logger.Info($"Call: {nameof(SendInactivityEmail)}(package)");
            return Send(new PackageTrackingCancelledEmailModel(package), EmailTypes.PostTrackTrackingCancellation);
        }

        private async Task Send(BaseEmailModel model, EmailTypes emailType)
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

        private async Task Transmit(string recipientEmail, Transmission transmission, EmailTypes emailType)
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