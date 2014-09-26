using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;
using belpost_checker.Properties;
using log4net;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net.Config;

namespace belpost_checker
{
    public class BelpostChecker
    {
        private static WebClient webClient = new WebClient();
        private static Uri url = new Uri("http://search.belpost.by/ajax/search");
        private static MongoCollection<TrackingDetail> mongoCollection;
        private static SmtpClient smtpClient = new SmtpClient(Settings.Default.SmtpHost, Settings.Default.SmtpPort);
        private static readonly ILog log = LogManager.GetLogger(typeof(BelpostChecker));

        public static void Main()
        {
            ConfigureLog4Net();
            log.Debug("Application started");
            try
            {
                webClient.Headers.Add("Accept", "text/html, */*");
                webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                webClient.Encoding = new UTF8Encoding();
                smtpClient.Credentials = new NetworkCredential(Settings.Default.SmtpUser, Settings.Default.SmtpPassword);
                smtpClient.EnableSsl = Settings.Default.SmtpSecure;
                mongoCollection = new MongoClient("mongodb://localhost").GetServer().GetDatabase("tracking").GetCollection<TrackingDetail>("packages");
                var items = mongoCollection.Find(Query.Or(Query<TrackingDetail>.NotExists(e => e.IsFinished), Query<TrackingDetail>.EQ(e => e.IsFinished, false))).ToList();
                if (items != null) Parallel.ForEach(items, x => ProcessItem(x));
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Operation error: {0} \n {1}", ex.Message, ex.StackTrace));
            }   
        }

        private static void ProcessItem(TrackingDetail model)
        {
            var httpResponseString = CheckStatus(model.Tracking);
            if (string.IsNullOrWhiteSpace(httpResponseString)) return;
            httpResponseString = Regex.Replace(httpResponseString, "<!--.*?-->", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            httpResponseString = Regex.Replace(httpResponseString, @"<[^>]*>", String.Empty);
            var matches = Regex.Matches(httpResponseString, @"[^\n](?<date>[^:\n]+:\d\d:\d\d)\n\s+(?<action>[^\n]+)\n(?<place>    [^\n]+)?");
            if (matches.Count == 0) return;
            var history = new SortedSet<PostDetailItem>();
            foreach (Match match in matches) history.Add(new PostDetailItem(match));
            if ((model.History == null && history.Any()) || (model.History.Length < history.Count))
            {
                var message = new MailMessage(Settings.Default.SmtpFrom, model.Email, GetEmailTitle(model), GetEmailBody(model, history));
                message.IsBodyHtml = true;
                smtpClient.Send(message);
                model.History = history.ToArray();
            }

            model.UpdateDate = DateTime.Now;
            model.IsFinished = model.History != null &&
                               model.History.Length > 0 &&
                               (model.History.Last().Action.Contains("Доставлено, вручено") || 
                               model.History.Last().Action == "Отправление доставлено");
            mongoCollection.Save(model);
        }

        private static string GetEmailTitle(TrackingDetail model)
        {
            return string.Format(Messages.StatusChancedEmailSubject, model.Description, model.Tracking);
        }

        private static string GetEmailBody(TrackingDetail model, ICollection<PostDetailItem> history)
        {
            var sb = new StringBuilder();
            foreach (var historyItem in history)
            {
                sb.Append(string.Format(
                    Messages.EmailBodyInner, 
                    historyItem.Date.ToString(), 
                    historyItem.Action + " " + historyItem.Place, 
                    model.History == null || model.History.All(x => x.Date != historyItem.Date) ? "changed" : string.Empty));
            }

            return string.Format(Messages.EmailBodyOuter, model.Description, model.Tracking).Replace("#INNER_TEMPLATE#", sb.ToString());
        }

        private static void ConfigureLog4Net()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var xml = new XmlDocument();
            xml.Load(Path.Combine(dir, "log4net.config"));
            XmlConfigurator.Configure((XmlElement)xml.DocumentElement);
        }

        private static string CheckStatus(string trackingNumber)
        {
            try { return webClient.UploadString(url, string.Format("item={0}&internal=2", trackingNumber)); }
            catch (WebException ex)
            {
                log.Error(string.Format("Checking tracking number {0}: {1} \n {2}", trackingNumber, ex.Message, ex.StackTrace));
                return null; 
            }
        }
    }
}