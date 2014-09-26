using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Posttrack.BLL
{
    public class UpdateStatusEventArgs : PackageEventArgs
    {
        private readonly ICollection<PackageHistoryItemDTO> history;

        public UpdateStatusEventArgs(PackageDTOWrapper wrapper, ICollection<PackageHistoryItemDTO> history) : base(wrapper)
        {
            this.history = history;
        }

        internal ICollection<PackageHistoryItemDTO> History
        {
            get
            {
                return this.history;
            }
        }
    }
}
