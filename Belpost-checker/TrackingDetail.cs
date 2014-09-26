using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace belpost_checker
{
    public class TrackingDetail
    {
        public MongoDB.Bson.BsonObjectId _id { get; set; }
        public string Email { get; set; }
        public string Tracking { get; set; }
        public string Description { get; set; }
        public PostDetailItem[] History { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsFinished { get; set; }
    }
}
