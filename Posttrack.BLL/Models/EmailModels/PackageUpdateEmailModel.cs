using Posttrack.Data.Interfaces.DTO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Posttrack.BLL.Models.EmailModels
{
    internal class PackageUpdateEmailModel : BaseEmailModel
    {
        internal readonly string Tracking;
        internal readonly string Description;
        internal readonly string Update;

        internal PackageUpdateEmailModel(PackageDTO package, IEnumerable<PackageHistoryItemDTO> update) : base(package.Email)
        {
            Tracking = package.Tracking;
            Description = package.Description;
            Update = LoadHistoryTemplate(package.History, update);
        }

        private static string LoadHistoryTemplate(
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
