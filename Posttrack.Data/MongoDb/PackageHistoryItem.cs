using System;

namespace Posttrack.Data.MongoDb
{
    public class PackageHistoryItem
    {
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public string Place { get; set; }
    }
}