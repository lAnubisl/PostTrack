using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Posttrack.Data.Interfaces.DTO;
using Posttrack.Data.MySql;

namespace Posttrack.Data
{
    internal static class Mapper
    {
        internal static Package Map(this RegisterPackageDTO model)
        {
            return new Package
            {
                Tracking = model.Tracking,
                Email = model.Email,
                Description = model.Description,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }

        internal static PackageDTO Map(this Package package)
        {
            return new PackageDTO
            {
                Email = package.Email,
                Description = package.Description,
                Tracking = package.Tracking,
                UpdateDate = package.UpdateDate,
                IsFinished = package.IsFinished,
                History = package.History.Map()
            };
        }

        internal static void Map(this PackageDTO dto, Package package)
        {
            package.IsFinished = dto.IsFinished;
            package.History = dto.History.Map();
        }

        private static ICollection<PackageHistoryItemDTO> Map(this string history)
        {
            return JsonConvert.DeserializeObject<ICollection<PackageHistoryItemDTO>>(history);
        }

        private static string Map(this IEnumerable<PackageHistoryItemDTO> history)
        {
            return JsonConvert.SerializeObject(history.ToList());
        }

        private static PackageHistoryItem Map(this PackageHistoryItemDTO item)
        {
            return new PackageHistoryItem {Action = item.Action, Place = item.Place, Date = item.Date};
        }

        private static PackageHistoryItemDTO Map(this PackageHistoryItem item)
        {
            return new PackageHistoryItemDTO {Action = item.Action, Date = item.Date, Place = item.Place};
        }
    }
}