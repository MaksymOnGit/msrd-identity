using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.PersistentStorage.Results.Identity
{
    public sealed class UnableToSetPasswordError : Error
    {
        public UnableToSetPasswordError() : base($"Error on user password setting.")
        {
            Metadata.Add("ErrorCode", nameof(UnableToSetPasswordError));
        }
    }
}
