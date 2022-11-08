using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Results.Account
{
    public sealed class RootUserLockoutNotAllowedError : Error
    {
        public RootUserLockoutNotAllowedError()
        : base($"Lockingout the root user is not allowed.")
        {
            Metadata.Add("ErrorCode", nameof(RootUserLockoutNotAllowedError));
        }
    }
}