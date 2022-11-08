using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    internal sealed class EmailOccupiedError : Error
    {
        public EmailOccupiedError(string email)
        : base($"User with this email already exists. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(EmailOccupiedError));
        }
    }
}
