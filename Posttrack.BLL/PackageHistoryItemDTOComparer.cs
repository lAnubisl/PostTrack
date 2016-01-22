using System.Collections.Generic;
using Posttrack.Data.Interfaces.DTO;

namespace Posttrack.BLL
{
    internal class PackageHistoryItemDTOComparer : IComparer<PackageHistoryItemDTO>
    {
        public int Compare(PackageHistoryItemDTO x, PackageHistoryItemDTO y)
        {
            if (x == null) return 1;
            if (y == null) return -1;
            if (x.Date == y.Date) return string.CompareOrdinal(x.Action, y.Action) * -1;
            return x.Date > y.Date ? -1 : 1;
        }
    }
}