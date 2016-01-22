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
            if (input.Contains("ничего не найдено"))
            {
                return null;
            }

            var matches = Regex.Matches(input, Settings.Default.HistoryRegex, RegexOptions.Singleline);
            if (matches.Count == 0)
            {
                log.Error("Cannot parse response string: " + input);
                return null;
            }

            var history = new SortedSet<PackageHistoryItemDTO>(comparer);
            foreach (Match match in matches)
            {
                var historyItem = new PackageHistoryItemDTO();
                historyItem.Date = ParseDate(match);
                historyItem.Action = match.Groups[2].Value.Trim();
	            if (match.Groups.Count > 3)
	            {
					historyItem.Place = match.Groups[4].Value.Trim();
				}
                
                history.Add(historyItem);
            }

            return history;
        }

	    private static DateTime ParseDate(Match match)
	    {
		    return match.Groups[1].Value.Contains("-")
			    ? DateTime.ParseExact(match.Groups[1].Value, "yyyy-MM-dd", provider)
			    : DateTime.ParseExact(match.Groups[1].Value, "dd.MM.yyyy", provider);
	    }
    }
}