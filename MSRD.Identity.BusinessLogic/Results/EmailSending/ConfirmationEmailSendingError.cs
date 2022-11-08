using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.EmailSending
{
    internal sealed class ConfirmationEmailSendingError : Error
    {
        public ConfirmationEmailSendingError(string email)
        : base($"Sending confirmation e-mail failed. Recipient: {email}")
        {
            Metadata.Add("ErrorCode", nameof(ConfirmationEmailSendingError));
        }
    }
}
