using System;

namespace Posttrack.Data.MySql
{
    public class PackageHistoryItem
    {
        public DateTime Date { get; set; }

        public string Action { get; set; }

        public string Place { get; set; }
    }
}