using Microsoft.AspNetCore.Identity;
using MSRD.Identity.Core.Auth.Models;

namespace MSRD.Identity.PersistentStorage.Stores
{
    public sealed class UserStore : Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserStore<User, Role, MsrdIdentityContext, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
    {
        public UserStore(MsrdIdentityContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}
