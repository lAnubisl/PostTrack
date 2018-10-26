using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Posttrack.BLL.Models.EmailModels
{
    internal abstract class BaseEmailModel
    {
        internal BaseEmailModel(string recipient)
        {
            Recipient = recipient;
        }

        internal string Year => DateTime.Now.Year.ToString();

        internal string Recipient { get; }

        protected static string LoadHistoryTemplate(
           ICollection<PackageHistoryItemDTO> oldHistory,
           IEnumerable<PackageHistoryItemDTO> newHistory)
        {
            var itemTemplate = @"<tr><td valign=""top"" style=""width: 140px; {Style}"">{Date}</td><td style = ""{Style}"">{Action} {Place}</td></tr>";
            var renderedItems = new Collection<string>();
            foreach (var item in newHistory)
            {
                var greenItem = oldHistory == null || !oldHistory.Contains(item);
                renderedItems.Add(itemTemplate
                    .Replace("{Date}", item.Date.ToString("dd.MM", CultureInfo.CurrentCulture))
                    .Replace("{Action}", CleanActionRegex.Replace(item.Action, string.Empty))
                    .Replace("{Place}", item.Place)
                    .Replace("{Style}", greenItem ? "color:green;font-weight:bold;" : string.Empty));
            }

            return string.Format("<table>{0}</table>", string.Join(string.Empty, renderedItems));
        }


        private static readonly Regex CleanActionRegex = new Regex("\\d{2}\\. ", RegexOptions.Compiled);
    }
}