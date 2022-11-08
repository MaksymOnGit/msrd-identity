using FluentResults;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Services.Interfaces
{
    public interface IAuthService
    {
        ValueTask<Result<ClaimsPrincipal>> AuthenticateUserAsync(string email, string password, ImmutableArray<string> scopes, CancellationToken cancellationToken = default);
        ValueTask<Result<ClaimsPrincipal>> RefreshTokenAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default);
    }
}
