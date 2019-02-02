using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Models.EmailModels
{
    internal abstract class BaseEmailModel
    {
        private static readonly Regex CleanActionRegex = new Regex("\\d{2}\\. ", RegexOptions.Compiled);

        internal BaseEmailModel(string recipient)
        {
            Recipient = recipient;
        }

        internal static string Year => DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture);

        internal string Recipient { get; }

        protected static string LoadHistoryTemplate(
           ICollection<PackageHistoryItemDTO> oldHistory,
           IEnumerable<PackageHistoryItemDTO> newHistory)
        {
            var itemTemplate = @"<tr><td valign=""top"" style=""width: 50px; {Style}"">{Date}</td><td style = ""{Style}"">{Action} {Place}</td></tr>";
            var renderedItems = new Collection<string>();
            if (newHistory != null)
            {
                foreach (var item in newHistory)
                {
                    var greenItem = oldHistory == null || !oldHistory.Contains(item);
                    renderedItems.Add(itemTemplate
                        .Replace("{Date}", item.Date.ToString("dd.MM", CultureInfo.InvariantCulture))
                        .Replace("{Action}", CleanActionRegex.Replace(item.Action, string.Empty))
                        .Replace("{Place}", item.Place)
                        .Replace("{Style}", greenItem ? "color:green;font-weight:bold;" : string.Empty));
                }
            }

            return $"<table>{string.Join(string.Empty, renderedItems)}</table>";
        }
    }
}