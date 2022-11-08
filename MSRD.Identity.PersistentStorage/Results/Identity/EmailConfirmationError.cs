using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.PersistentStorage.Results.Identity
{
    public sealed class EmailConfirmationError : Error
    {
        public EmailConfirmationError(string email) : base($"Error on email confirmation. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(UserCreationError));
        }
    }
}
