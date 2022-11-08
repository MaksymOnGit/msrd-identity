using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    public sealed class InvalidUserIdentifierError : Error
    {
        public InvalidUserIdentifierError(string email)
        : base($"User with this email address is not exists. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(InvalidUserIdentifierError));
        }
    }
}
