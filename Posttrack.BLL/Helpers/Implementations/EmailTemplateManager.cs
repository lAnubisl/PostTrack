using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using System;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class EmailTemplateManager : IEmailTemplateManager
    {
        string IEmailTemplateManager.GetRegisteredEmailBody(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            return LoadInnerHtml("TemplateInnerRegistered.html", package.Description, package.Tracking)
                .Replace("{RegisterStatusMessage}", LoadRegisteredItemsUpdate(package, update));
        }

        string IEmailTemplateManager.GetInactivityEmailBody(PackageDTO package)
        {
            return LoadInnerHtml("TemplateInnerInactivity.html", package.Description, package.Tracking);
        }

        string IEmailTemplateManager.GetUpdateStatusEmailBody(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            return LoadInnerHtml("TemplateInnerStatusChanged.html", package.Description, package.Tracking)
                .Replace("{TemplateHistoryItems}", LoadHistoryTemplate(package.History, update));
        }

        private static string LoadRegisteredItemsUpdate(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update)
        {
            if (PackageHelper.IsEmpty(update))
            {
                return EmailMessages.RegisterStatusNotFoundMessage;
            }
            else
            {
                return LoadHistoryTemplate(package.History, update);
            }
        }

        private static string LoadInnerHtml(string templateName, string description, string tracking)
        {
            var template = LoadTemplate("TemplateOuter.html")
                .Replace("{TemplateInner}", LoadTemplate(templateName));
            return template
                .Replace("{Description}", description)
                .Replace("{Tracking}", tracking);
        }

        private static string LoadHistoryTemplate(
            ICollection<PackageHistoryItemDTO> oldHistory,
            IEnumerable<PackageHistoryItemDTO> newHistory)
        {
            var itemTemplate = LoadTemplate("TemplateItem.html");
            var sb = new StringBuilder();
            var oldItemsCount = oldHistory != null ? oldHistory.Count : 0;
            var counter = 1;
            foreach (var item in newHistory)
            {
                sb.Append(itemTemplate
                    .Replace("{Date}", item.Date.ToString("dd.MM.yy HH:mm", CultureInfo.CurrentCulture))
                    .Replace("{Action}", item.Action)
                    .Replace("{Place}", item.Place)
                    .Replace("{Style}", counter > oldItemsCount ? "color:green;font-weight:bold;" : string.Empty));
                counter++;
            }

            return string.Format("<table>{0}</table>", sb.ToString());
        }

        private static string LoadTemplate(string templateName)
        {
            return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", templateName));
        }
    }
}