using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Auth
{
    public sealed class InvalidAuthenticationData : Error
    {
        public InvalidAuthenticationData()
        : base($"The provided email or password is invalid.")
        {
            Metadata.Add("ErrorCode", nameof(InvalidAuthenticationData));
        }
    }
}
