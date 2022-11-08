using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSRD.Identity.Core.Auth.Models
{
    public class User : IdentityUser<string>
    {
        public User() : base()
        {
        }
        public User(string email)
        {
            Email = email;
            UserName = email;
        }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}
