using Microsoft.AspNetCore.Identity;

namespace MSRD.Identity.Core.Auth.Models
{
    public class Role : IdentityRole
    {
        public Role() : base()
        {
        }
        public Role(string roleName) : base(roleName)
        {
        }
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
