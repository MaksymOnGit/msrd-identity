using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MSRD.Identity.BusinessLogic.Results.Auth;
using MSRD.Identity.BusinessLogic.Services.Interfaces;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.PersistentStorage.Repositories.Interfaces;
using OpenIddict.Abstractions;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MSRD.Identity.BusinessLogic.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IIdentityRepository identityRepository;
        private readonly SignInManager<User> signInManager;

        public AuthService(IIdentityRepository identityRepository, SignInManager<User> signInManager)
        {
            this.identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(IIdentityRepository));
            this.signInManager = signInManager;
        }

        public async ValueTask<Result<ClaimsPrincipal>> AuthenticateUserAsync(string email, string password, ImmutableArray<string> scopes, CancellationToken cancellationToken = default)
        {

            var user = await identityRepository.FindUserByEmailAsync(email);
            if (user == null)
                return Result.Fail(new InvalidAuthenticationData());

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return Result.Fail(new InvalidAuthenticationData());

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.AddClaims(new[]
            {
                new Claim(Claims.Subject, user.Id),
                new Claim(Claims.Email, user.Email),
            });

            foreach (var role in await signInManager.UserManager.GetRolesAsync(user))
            {
                identity.AddClaim(new Claim(Claims.Role, role));
            }

            var principal = new ClaimsPrincipal(identity);

            principal.SetScopes(new[]
            {
                Scopes.OpenId,
                Scopes.Email,
                Scopes.OfflineAccess
            }.Intersect(scopes));

            SetDestinations(principal);

            return Result.Ok(principal);
        }

        private static void SetDestinations(ClaimsPrincipal principal)
        {

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(claim.Type switch
                {
                    var x when x is Claims.Email && principal.HasScope(Scopes.Email) => new[]
                    {
                        Destinations.IdentityToken
                    },
                    var x when x is Claims.Role => new[] { Destinations.AccessToken },
                    "secret_value" => Array.Empty<string>(),
                    "AspNet.Identity.SecurityStamp" => Array.Empty<string>(),
                    _ => null
                });
            }
        }

        public async ValueTask<Result<ClaimsPrincipal>> RefreshTokenAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            var user = await signInManager.UserManager.FindByIdAsync(principal.GetClaim(Claims.Subject));
            if (user == null)
            {
                return Result.Fail(new InvalidAuthenticationData());
            }

            if (!await signInManager.CanSignInAsync(user))
            {
                return Result.Fail(new InvalidAuthenticationData());
            }

            SetDestinations(principal);

            return Result.Ok(principal);
        }
    }
}
