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

namespace Posttrack.BLL.Helpers.Implementations
{
    public class EmailMessageSender : IMessageSender
    {
        private readonly ISparkPostTemplateProvider _sparkPostTemplateProvider;
        private readonly string _apiKey;

        public EmailMessageSender(IConfigurationService settingsProvider, ISparkPostTemplateProvider sparkPostTemplateProvider)
        {
            _apiKey = settingsProvider.SparkPostApiKey;
            _sparkPostTemplateProvider = sparkPostTemplateProvider;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public Task SendStatusUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            return Send(new PackageUpdateEmailModel(package, update), EmailTypes.PostTrackPackageUpdate);
        }

        public Task SendRegistered(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            return Send(new PackageRegisteredEmailModel(package), EmailTypes.PostTrackRegistered);
        }

        public Task SendInactivityEmail(PackageDTO package)
        {
            return Send(new PackageTrackingCancelledEmailModel(package), EmailTypes.PostTrackTrackingCancellation);
        }

        private async Task Send(BaseEmailModel model, EmailTypes emailType)
        {
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
            try
            {
                transmission.Recipients.Add(new Recipient
                {
                    Address = new Address { Email = recipientEmail }
                });
                var client = new Client(_apiKey);
                await client.Transmissions.Send(transmission);
                //logger.Info($"Message {emailType} sent to {recipientEmail}");
            }
            catch (Exception ex)
            {
                //logger.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}