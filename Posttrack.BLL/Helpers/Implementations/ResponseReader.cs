using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using log4net;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.BLL.Properties;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class ResponseReader : IResponseReader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (ResponseReader));
        private static readonly CultureInfo provider = new CultureInfo("ru-RU");
        private static readonly PackageHistoryItemDTOComparer comparer = new PackageHistoryItemDTOComparer();

        ICollection<PackageHistoryItemDTO> IResponseReader.Read(string input)
        {
            if (input.StartsWith("По вашему запросу ничего не найдено"))
            {
                return null;
            }

            input = Regex.Replace(input, "<!--.*?-->", string.Empty,
                RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            input = Regex.Replace(input, @"<[^>]*>", String.Empty);
            MatchCollection matches = Regex.Matches(input, Settings.Default.HistoryRegex);
            if (matches.Count == 0)
            {
                log.Error("Cannot parse response string: " + input);
                return null;
            }

            var history = new SortedSet<PackageHistoryItemDTO>(comparer);
            foreach (Match match in matches)
            {
                var historyItem = new PackageHistoryItemDTO();
                historyItem.Date = DateTime.Parse(match.Groups["date"].Value.Trim(), provider);
                historyItem.Action = match.Groups["action"].Value.Trim();
                historyItem.Place = match.Groups["place"].Value.Trim();
                history.Add(historyItem);
            }

            return history;
        }
    }
}