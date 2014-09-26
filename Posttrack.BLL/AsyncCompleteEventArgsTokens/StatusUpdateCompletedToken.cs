using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Posttrack.BLL.AsyncCompleteEventArgsTokens
{
    internal class StatusUpdateCompletedToken : SendEmailToken
    {
        private readonly PackageDTOWrapper wrapper;
        private readonly ICollection<PackageHistoryItemDTO> update;

        internal StatusUpdateCompletedToken(MailMessage message, PackageDTOWrapper wrapper, ICollection<PackageHistoryItemDTO> update)
            : base(message)
        {
            this.wrapper = wrapper;
            this.update = update;
        }

        internal PackageDTOWrapper Wrapper { get { return this.wrapper; } }
        internal ICollection<PackageHistoryItemDTO> Update { get { return this.update; } }
    }
}
