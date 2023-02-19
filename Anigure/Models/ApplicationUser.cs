using Microsoft.AspNetCore.Identity;

namespace Anigure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<IdentityUserRole<string>> Roles { get; set; } = null!;
    }
}
