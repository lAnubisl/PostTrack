using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Posttrack.BLL
{
    public class PackageDTOWrapper
    {
        private readonly PackageDTO package;
        private readonly AutoResetEvent completeTrigger;

        internal PackageDTOWrapper(PackageDTO package, AutoResetEvent completeTrigger)
        {
            this.package = package;
            this.completeTrigger = completeTrigger;
        }

        internal PackageDTO Package
        {
            get { return this.package; }
        }

        internal void NotifyAsyncComplete()
        {
            this.NotifyAsyncComplete();
        }
    }
}