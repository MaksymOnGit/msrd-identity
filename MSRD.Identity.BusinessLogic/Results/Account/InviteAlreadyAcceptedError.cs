using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    public sealed class InviteAlreadyAcceptedError : Error
    {
        public InviteAlreadyAcceptedError(string email)
        : base($"User with this email already accepted the invitation and registered an account. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(InviteAlreadyAcceptedError));
        }
    }
}
