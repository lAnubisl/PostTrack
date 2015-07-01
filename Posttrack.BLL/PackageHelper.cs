using Posttrack.BLL.Properties;
using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

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

            string historyAction = package.History.First().Action;
            return historyAction != null &&
                                     (historyAction.ToLowerInvariant().Contains("вручено") ||
                                      historyAction == "Отправление доставлено");
        }

        internal static bool IsInactivityPeriodElapsed(PackageDTO package)
        {
            return package.UpdateDate <= DateTime.Now.AddMonths(-Settings.Default.InactivityPeriodInMonths);
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
