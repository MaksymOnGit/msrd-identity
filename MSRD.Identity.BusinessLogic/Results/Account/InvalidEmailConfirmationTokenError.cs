using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    public sealed class InvalidEmailConfirmationTokenError : Error
    {
        public InvalidEmailConfirmationTokenError()
        : base($"Invalid email confirmation token.")
        {
            Metadata.Add("ErrorCode", nameof(InvalidEmailConfirmationTokenError));
        }
    }
}
