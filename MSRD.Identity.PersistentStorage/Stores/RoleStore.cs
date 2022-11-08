using Microsoft.AspNetCore.Identity;
using MSRD.Identity.Core.Auth.Models;

namespace MSRD.Identity.PersistentStorage.Stores
{
    public sealed class RoleStore : Microsoft.AspNetCore.Identity.EntityFrameworkCore.RoleStore<Role, MsrdIdentityContext, string, UserRole, IdentityRoleClaim<string>>
    {
        public RoleStore(MsrdIdentityContext context, IdentityErrorDescriber? describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}
