using System;
using System.Collections.Generic;
using System.Linq;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL
{
    internal static class PackageHelper
    {
        internal static bool IsFinished(PackageDTO package)
        {
            if (IsEmpty(package.History))
            {
                return false;
            }

            var historyAction = package.History.First().Action;
            return historyAction != null &&
#pragma warning disable CA1308 // Normalize strings to uppercase
                   (historyAction.ToLowerInvariant().Contains("вручено") ||
#pragma warning restore CA1308 // Normalize strings to uppercase
                    historyAction == "Отправление доставлено");
        }

        internal static bool IsInactivityPeriodElapsed(PackageDTO package, int inactivityPeriodMonths)
        {
            return package.UpdateDate <= DateTime.Now.AddMonths(-inactivityPeriodMonths);
        }

        internal static bool IsStatusTheSame(ICollection<PackageHistoryItemDTO> history, PackageDTO package)
        {
            if (IsEmpty(history) && IsEmpty(package.History))
            {
                return true;
            }

            if (!IsEmpty(history) && !IsEmpty(package.History) && package.History.Count == history.Count)
            {
                return true;
            }

            return false;
        }

        internal static bool IsEmpty(IEnumerable<PackageHistoryItemDTO> history)
        {
            return history == null || !history.Any();
        }
    }
}