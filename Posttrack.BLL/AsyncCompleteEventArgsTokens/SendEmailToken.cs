using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Posttrack.BLL.AsyncCompleteEventArgsTokens
{
    internal class SendEmailToken
    {
        private readonly MailMessage message;

        internal SendEmailToken(MailMessage message)
        {
            this.message = message;
        }

        internal void DisposeMailMessage()
        {
            if (message != null)
            {
                message.Dispose();
            }
        }
    }
}