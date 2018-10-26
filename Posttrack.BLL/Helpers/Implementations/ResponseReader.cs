﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Posttrack.BLL.Helpers.Interfaces;
using Posttrack.Data.Interfaces.DTO;
using Posttrack.BLL.Interfaces;
using Posttrack.Common;

namespace Posttrack.BLL.Helpers.Implementations
{
    public class ResponseReader : IResponseReader
    {
        private readonly ILogger _logger;
        private static readonly CultureInfo provider = new CultureInfo("ru-RU");
        private readonly IConfigurationService _configurationService;
        private static readonly PackageHistoryItemDTOComparer comparer = new PackageHistoryItemDTOComparer();

        public ResponseReader(IConfigurationService configurationService, ILogger logger)
        {
            _logger = logger.CreateScope(nameof(ResponseReader));
            _configurationService = configurationService;
        }

        public ICollection<PackageHistoryItemDTO> Read(string input)
        {
            _logger.Info($"Call: {nameof(Read)}()");
            if (input.Contains("ничего не найдено"))
            {
                return null;
            }

            var matches = Regex.Matches(input, _configurationService.HistoryRegex, RegexOptions.Singleline);
            if (matches.Count == 0)
            {
                _logger.Error($"Cannot parse response string: {input}.");
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