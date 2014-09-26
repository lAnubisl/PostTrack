using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Posttrack.BLL
{
    public class PackageEventArgs : EventArgs
    {
        private readonly PackageDTOWrapper wrapper;

        public PackageEventArgs(PackageDTOWrapper wrapper) : base()
        {
            this.wrapper = wrapper;
        }

        internal PackageDTOWrapper Wrapper
        {
            get
            {
                return this.wrapper;
            }
        }
    }
}