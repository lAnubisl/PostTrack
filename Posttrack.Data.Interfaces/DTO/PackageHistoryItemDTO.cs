using System;

namespace Posttrack.Data.Interfaces.DTO
{
    public class PackageHistoryItemDTO
    {
        public DateTime Date { get; set; }

        public string Action { get; set; }

        public string Place { get; set; }
    }
}