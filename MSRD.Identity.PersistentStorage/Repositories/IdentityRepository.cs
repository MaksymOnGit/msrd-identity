using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MSRD.Identity.Core;
using MSRD.Identity.Core.Auth.Models;
using MSRD.Identity.Core.Query;
using MSRD.Identity.PersistentStorage.Repositories.Interfaces;
using MSRD.Identity.PersistentStorage.Results.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MSRD.Identity.PersistentStorage.Repositories
{
    public sealed class IdentityRepository : IIdentityRepository
    {
        private readonly MsrdIdentityContext authContext;
        private readonly Microsoft.AspNetCore.Identity.UserManager<User> userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager;
        private readonly AdminUser adminUser;

        public IdentityRepository(MsrdIdentityContext authContext, Microsoft.AspNetCore.Identity.UserManager<User> userManager, Microsoft.AspNetCore.Identity.RoleManager<Role> roleManager, IOptions<AdminUser> adminUser)
        {
            this.authContext = authContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.adminUser = adminUser.Value;
        }

        public async ValueTask<User> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await userManager.FindByEmailAsync(email);
        }

        public async ValueTask<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async ValueTask<Result> ConfirmEmailAsync(User user, string token, CancellationToken cancellationToken = default)
        {
            var identityResult = await userManager.ConfirmEmailAsync(user, token);
            return Result.OkIf(identityResult.Succeeded, new EmailConfirmationError(user.Email).CausedBy(identityResult.Errors.Select(x => new Error(x.Description).WithMetadata("ErrorCode", x.Code))));

        }

        public void AddToRole(User user, Role role, CancellationToken cancellationToken = default)
        {
            user.UserRoles.Add(new UserRole { Role = role, User = user });
        }

        public async Task SeedUsersAndRoles()
        {
            var role = await roleManager.FindByNameAsync("Admin");
            if (role is null)
            {
                role = new Role("Admin");
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                    throw new Exception("Admin role creation unsuccessful");
            }

            //TODO: ensure not empty email and password properties
            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user is null)
            {
                user = new User(adminUser.Email);
                user.EmailConfirmed = true;
                user.Id = Guid.NewGuid().ToString();
                var userResult = await userManager.CreateAsync(user, adminUser.Password);
                if (!userResult.Succeeded)
                    throw new Exception("Admin user creation unsuccessful");
            }
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                AddToRole(user, role);
            }

            await SaveChangesAsync();
        }

        public async ValueTask SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await authContext.SaveChangesAsync(cancellationToken);
        }

        public async ValueTask<bool> IsEmailOccupiedAsync(string email, CancellationToken cancellationToken = default)
        {
            return await authContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        }

        public async ValueTask<Result> CrateEmptyUserAsync(string email, CancellationToken cancellationToken = default)
        {
            var identityResult = await userManager.CreateAsync(new User { Id = Guid.NewGuid().ToString(), Email = email, UserName = email });
            return Result.OkIf(identityResult.Succeeded, new UserCreationError(email).CausedBy(identityResult.Errors.Select(x => new Error(x.Description).WithMetadata("ErrorCode", x.Code))));
        }

        public async ValueTask<Result> SetUserPasswordAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            var identityResult = await userManager.AddPasswordAsync(user, password);
            return Result.OkIf(identityResult.Succeeded, new UnableToSetPasswordError().CausedBy(identityResult.Errors.Select(x => new Error(x.Description).WithMetadata("ErrorCode", x.Code))));
        }

        public async ValueTask<QueryResponse<UserInfoView>> QueryUsersNoTrackingAsync(QueryRequest queryRequest, CancellationToken cancellationToken = default)
        {
            
            var response = new QueryResponse<UserInfoView>();

            response.TotalRecordsCount = await userManager.Users.CountAsync(cancellationToken);
            response.TotalPagesCount = response.TotalRecordsCount / queryRequest.Rows;
            var currentRequestedPage = queryRequest.Offset / queryRequest.Rows;
            response.Page = currentRequestedPage < response.TotalPagesCount ? currentRequestedPage + 1 : response.TotalPagesCount + 1;
            response.RecordsPerPageCount = queryRequest.Rows;
            response.IsPrev = response.Page > 1;
            response.IsNext = response.Page < response.TotalRecordsCount;

            var query = userManager.Users.AsNoTracking()
                .Skip(queryRequest.Offset)
                .Take(queryRequest.Rows);

            switch (queryRequest.SortField?.ToLower())
            {
                case "email":
                    query = queryRequest.SortOrder < 0 ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email);
                    break;
                case "emailconfirmed":
                    query = queryRequest.SortOrder < 0 ? query.OrderByDescending(x => x.EmailConfirmed) : query.OrderBy(x => x.EmailConfirmed);
                    break;
                case "id":
                    query = queryRequest.SortOrder < 0 ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.LockoutEnabled);
                    break;
                case "lockoutend":
                    query = queryRequest.SortOrder < 0 ? query.OrderByDescending(x => x.LockoutEnd) : query.OrderBy(x => x.LockoutEnd);
                    break;
                case "isadmin":
                    query = queryRequest.SortOrder < 0 ? query.OrderByDescending(x => x.UserRoles.Any(y => y.Role.NormalizedName == "ADMIN")) : query.OrderBy(x => x.UserRoles.Any(y => y.Role.NormalizedName == "ADMIN"));
                    break;
                default:
                    query = query.OrderBy(x => x.Email);
                    break;
            }

            response.Result = await query.Include(x => x.UserRoles).ThenInclude(x => x.Role).Select(x => new UserInfoView
            {
                Email = x.Email,
                Id = x.Id,
                EmailConfirmed = x.EmailConfirmed,
                LockoutEnd = x.LockoutEnd,
                IsAdmin = x.UserRoles.Any(y => y.Role.NormalizedName == "ADMIN")
            }).ToListAsync(cancellationToken);

            return response;
        }
    }
}

