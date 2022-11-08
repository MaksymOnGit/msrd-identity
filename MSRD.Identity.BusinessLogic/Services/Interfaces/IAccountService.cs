using FluentResults;
using Microsoft.AspNetCore.Identity;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.Core.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Services.Interfaces
{
    public interface IAccountService
    {
        ValueTask<Result> InviteNewUserAsync(string email, CancellationToken cancellationToken = default);
        ValueTask<QueryResponse<UserInfoView>> QueryUsersNoTrackingAsync(QueryRequest queryRequest, CancellationToken cancellationToken = default);
        ValueTask<Result> RegisterNewUserAsync(string email, string token, string password, CancellationToken cancellationToken = default);
        ValueTask<Result> ResendInvitationEmailAsync(string email, CancellationToken cancellationToken = default);
        ValueTask<Result> SetUserLockoutAsync(string email, DateTime? until, CancellationToken cancellationToken = default);
    }
}
