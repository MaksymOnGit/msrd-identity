using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.PersistentStorage.Results.Identity
{
    public sealed class UserCreationError : Error
    {
        public UserCreationError(string email) : base($"Error on user creation. Email: {email}")
        {
            Metadata.Add("ErrorCode", nameof(UserCreationError));
        }
    }
}
