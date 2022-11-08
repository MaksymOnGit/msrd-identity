using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    public sealed class EmailNotBeenYetInvitedError : Error
    {
        public EmailNotBeenYetInvitedError(string email)
        : base($"User with this email address has not been invited yet. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(EmailNotBeenYetInvitedError));
        }
    }
}
