using FluentResults;
using Microsoft.Extensions.Options;
using MSRD.Identity.BusinessLogic.Results.Account;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using MSRD.Identity.Core;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.Core.Query;
using MSRD.Identity.PersistentStorage.Repositories;
using MSRD.Identity.PersistentStorage.Repositories.Interfaces;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.BusinessLogic.Services
{
    public sealed class AccountService : IAccountService
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IEmailSendingService emailSendingService;
        private readonly AdminUser adminUser;

        public AccountService(IIdentityRepository identityRepository, IEmailSendingService emailSendingService, IOptionsSnapshot<AdminUser> adminUser)
        {
            this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(IIdentityRepository));
            this.emailSendingService = emailSendingService ?? throw new ArgumentNullException(nameof(IEmailSendingService));
            this.adminUser = adminUser.Value;
        }

        public async ValueTask<Result> RegisterNewUserAsync(string email, string token, string password, CancellationToken cancellationToken = default)
        {
            var user = await identityRepository.FindUserByEmailAsync(email);

            if (user is null)
                return Result.Fail(new EmailNotBeenYetInvitedError(email));


            var tokenDecodeResult = Result.Try(() => Encoding.UTF8.GetString(Convert.FromBase64String(token)), ex => new InvalidEmailConfirmationTokenError());

            if (tokenDecodeResult.IsFailed)
                return tokenDecodeResult.ToResult();

            var result = await identityRepository.ConfirmEmailAsync(user, tokenDecodeResult.Value, cancellationToken);

            if (result.IsFailed)
                return result;

            await identityRepository.SaveChangesAsync(cancellationToken);

            result = await identityRepository.SetUserPasswordAsync(user, password, cancellationToken);

            if (result.IsFailed)
                return result;

            await identityRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }

        public async ValueTask<Result> InviteNewUserAsync(string email, CancellationToken cancellationToken = default)
        {

            var emailOccupied = await identityRepository.IsEmailOccupiedAsync(email, cancellationToken);

            if (emailOccupied)
            {
                return Result.Fail(new EmailOccupiedError(email));
            }

            var result = await identityRepository.CrateEmptyUserAsync(email, cancellationToken);

            if (result.IsFailed)
            {
                return result;
            }

            await identityRepository.SaveChangesAsync(cancellationToken);

            var user = await identityRepository.FindUserByEmailAsync(email, cancellationToken);
            var token = await identityRepository.GenerateEmailConfirmationTokenAsync(user, cancellationToken);


            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
            _ = emailSendingService.SendRegistrationConfirmationEmailAsync(user.Email, token, cancellationToken);

            return Result.Ok();
        }

        public async ValueTask<Result> ResendInvitationEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await identityRepository.FindUserByEmailAsync(email, cancellationToken);

            if (user is null)
                return Result.Fail(new EmailNotBeenYetInvitedError(email));

            if (user.EmailConfirmed)
                return Result.Fail(new InviteAlreadyAcceptedError(email));

            var token = await identityRepository.GenerateEmailConfirmationTokenAsync(user, cancellationToken);
            token = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

            return await emailSendingService.SendRegistrationConfirmationEmailAsync(user.Email, token, cancellationToken);
        }

        public async ValueTask<QueryResponse<UserInfoView>> QueryUsersNoTrackingAsync(QueryRequest queryRequest, CancellationToken cancellationToken = default) => await identityRepository.QueryUsersNoTrackingAsync(queryRequest, cancellationToken);

        public async ValueTask<Result> SetUserLockoutAsync(string email, DateTime? until, CancellationToken cancellationToken = default)
        {
            var user = await identityRepository.FindUserByEmailAsync(email, cancellationToken);

            if (user is null)
                return Result.Fail(new InvalidUserIdentifierError(email));

            if (user.Email == adminUser.Email && until is not null)
                return Result.Fail(new RootUserLockoutNotAllowedError());

            user.LockoutEnd = until;

            await identityRepository.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
