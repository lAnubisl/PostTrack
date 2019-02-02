using System;

namespace Posttrack.Data.MySql
{
    internal class Package
    {
        public string Email { get; set; }

        public string Tracking { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool IsFinished { get; set; }

        public string History { get; set; }
    }
}