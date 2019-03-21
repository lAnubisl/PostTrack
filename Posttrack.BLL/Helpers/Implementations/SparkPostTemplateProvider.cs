using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;
using SparkPost;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class SparkPostTemplateProvider : ISparkPostTemplateProvider
    {
        private static readonly ConcurrentDictionary<EmailType, TemplateHolder> Templates = new ConcurrentDictionary<EmailType, TemplateHolder>();
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromHours(1);
        private readonly string _apiKey;
        private readonly ILogger _logger;

        public SparkPostTemplateProvider(ISettingsService settingsProvider, ILogger logger)
        {
            _apiKey = settingsProvider.SparkPostApiKey;
            _logger = logger.CreateScope(nameof(SparkPostTemplateProvider));
        }

        public string GetTemplateId(EmailType type)
        {
            var typeName = Enum.GetName(typeof(EmailType), type);
            if (typeName == null) return null;
            typeName = Regex.Replace(typeName, "(?<=.)([A-Z])", "-$0", RegexOptions.Compiled);
#pragma warning disable CA1308 // Normalize strings to uppercase
            return typeName.ToLower(CultureInfo.InvariantCulture);
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        public async Task<RetrieveTemplateResponse> GetTemplate(EmailType type)
        {
            _logger.Info($"GetTemplate({type})");
            Templates.TryGetValue(type, out TemplateHolder templateHolder);
            if (templateHolder != null && templateHolder.LastUpdate.Add(UpdateInterval) >= DateTime.UtcNow)
            {
                return templateHolder.Template;
            }

            var client = new Client(_apiKey);
            var emailTemplate = await client.Templates.Retrieve(GetTemplateId(type));
            Templates.AddOrUpdate(
                type,
                (types) => new TemplateHolder(emailTemplate),
                (types, holder) =>
                {
                    holder.Template = emailTemplate;
                    return holder;
                });

            return emailTemplate;
        }

        private class TemplateHolder
        {
            private RetrieveTemplateResponse template;

            public TemplateHolder(RetrieveTemplateResponse template)
            {
                Template = template;
                LastUpdate = DateTime.UtcNow;
            }

            public DateTime LastUpdate { get; private set; }

            public RetrieveTemplateResponse Template
            {
                get
                {
                    return template;
                }

                set
                {
                    LastUpdate = DateTime.UtcNow;
                    template = value;
                }
            }
        }
    }
}