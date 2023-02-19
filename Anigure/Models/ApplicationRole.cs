using Microsoft.AspNetCore.Identity;

namespace Anigure.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
