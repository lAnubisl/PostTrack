using Posttrack.Data.Interfaces.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Posttrack.BLL.AsyncCompleteEventArgsTokens
{
    internal sealed class WebRequestToken
    {
        private readonly WebClient webClient;
        private readonly PackageDTOWrapper wrapper;

        internal WebRequestToken(WebClient webClient, PackageDTOWrapper wrapper)
        {
            this.webClient = webClient;
            this.wrapper = wrapper;
        }

        internal PackageDTOWrapper Wrapper
        {
            get { return this.wrapper; }
        }

        internal void DisposeWebClient()
        {
            if (webClient != null)
            {
                webClient.Dispose();
            }
        }
    }
}
