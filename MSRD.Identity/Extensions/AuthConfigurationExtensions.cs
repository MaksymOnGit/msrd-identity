using Microsoft.AspNetCore.Identity;
using MSRD.Identity.Core;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.PersistentStorage;
using MSRD.Identity.PersistentStorage.Stores;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MSRD.Identity.Extensions
{
    public static class AuthConfigurationExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<User>(opt => opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider)
                .AddRoles<Role>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<MsrdIdentityContext>()
                .AddUserStore<UserStore>()
                .AddUserManager<UserManager<User>>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddict.Abstractions.OpenIddictConstants.Claims.Role;

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options
                        .UseEntityFrameworkCore()
                        .UseDbContext<MsrdIdentityContext>();
                })
                .AddServer(options =>
                {
                    options
                        .SetTokenEndpointUris("/connect/token")
                        .SetRefreshTokenLifetime(TimeSpan.FromHours(1))
                        .SetAccessTokenLifetime(TimeSpan.FromSeconds(10))
                        .AllowPasswordFlow()
                        .AllowRefreshTokenFlow()
                        .AcceptAnonymousClients()
                        .RegisterScopes(new[] { Scopes.Email })
                        .DisableAccessTokenEncryption()
                        .UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .DisableTransportSecurityRequirement();

                    options.AddDevelopmentEncryptionCertificate();
                    options.AddDevelopmentSigningCertificate();

                    options.SetAccessTokenLifetime(new TimeSpan(0, 5, 0));
                    options.SetRefreshTokenLifetime(null);
                }).AddValidation(options =>
                {
                    // Note: the validation handler uses OpenID Connect discovery
                    // to retrieve the address of the introspection endpoint.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            return services;
        }
    }
}
