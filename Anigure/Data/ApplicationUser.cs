using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<IdentityUserRole<string>> Roles { get; set; } = null!;
        public bool IsBlocked { get; set; }
    }
}
