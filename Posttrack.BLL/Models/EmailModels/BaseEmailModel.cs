using System;

namespace Posttrack.BLL.Models.EmailModels
{
    internal abstract class BaseEmailModel
    {
        internal BaseEmailModel(string recipient)
        {
            Recipient = recipient;
        }

        internal string Year => DateTime.Now.Year.ToString();

        internal string Recipient { get; }
    }
}