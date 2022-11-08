using AutoMapper;
using MSRD.Identity.Core.Auth.Models;

namespace MSRD.Identity.Models.UserManagement.Responses
{
    public sealed class UserResponseModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsAdmin { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
