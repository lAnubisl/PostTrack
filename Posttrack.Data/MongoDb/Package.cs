using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Posttrack.Data.MongoDb
{
    internal class Package
    {
        public BsonObjectId _id { get; set; }
        public string Email { get; set; }
        public string Tracking { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public ICollection<PackageHistoryItem> History { get; set; }
        public bool IsFinished { get; set; }
    }
}