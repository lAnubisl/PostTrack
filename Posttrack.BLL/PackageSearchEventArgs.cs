using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Posttrack.BLL
{
    public class PackageSearchEventArgs : PackageEventArgs
    {
        private readonly string searchResult;

        public PackageSearchEventArgs(PackageDTOWrapper wrapper, string searchResult) : base(wrapper)
        {
            this.searchResult = searchResult;
        }

        internal string SearchResult
        {
            get { return this.searchResult; }
        }
    }
}