using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Interfaces;
using SparkPost;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class SparkPostTemplateProvider : ISparkPostTemplateProvider
    {
        //private readonly ILogger logger;
        private static ConcurrentDictionary<EmailTypes, TemplateHolder> templates;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromHours(1);
        private readonly string _apiKey;
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

        public SparkPostTemplateProvider(IConfigurationService settingsProvider)
        {
            _apiKey = settingsProvider.SparkPostApiKey;
            //this.logger = logger.CreateScope(nameof(SparkPostTemplateProvider));
        }

        static SparkPostTemplateProvider()
        {
            templates = new ConcurrentDictionary<EmailTypes, TemplateHolder>();
        }

        public string GetTemplateId(EmailTypes type)
        {
            var typeName = Enum.GetName(typeof(EmailTypes), type);
            if (typeName == null) return null;
            typeName = Regex.Replace(typeName, "(?<=.)([A-Z])", "-$0", RegexOptions.Compiled);
            return typeName.ToLower();
        }

        public async Task<RetrieveTemplateResponse> GetTemplate(EmailTypes type)
        {
            TemplateHolder templateHolder;
            templates.TryGetValue(type, out templateHolder);
            if (templateHolder != null && templateHolder.LastUpdate.Add(UpdateInterval) >= DateTime.UtcNow)
            {
                return templateHolder.Template;
            }

            //logger.Info($"LoadTemplate({type})");
            var client = new Client(_apiKey);
            var emailTemplate = await client.Templates.Retrieve(GetTemplateId(type));
            templates.AddOrUpdate(type,
                (types) => new TemplateHolder(emailTemplate),
                (types, holder) =>
                {
                    holder.Template = emailTemplate;
                    return holder;
                });

            return emailTemplate;
        }
    }
}