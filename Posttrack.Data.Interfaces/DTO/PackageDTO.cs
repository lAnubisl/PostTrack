using System;
using System.Collections.Generic;

namespace Posttrack.Data.Interfaces.DTO
{
    public class PackageDTO
    {
        public string Description { get; set; }

        public string Email { get; set; }

        public string Tracking { get; set; }

        public DateTime UpdateDate { get; set; }

        public ICollection<PackageHistoryItemDTO> History { get; set; }

        public bool IsFinished { get; set; }
    }
}