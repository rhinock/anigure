using Anigure.Data.Base;
using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationUser : IdentityUser, IResource
    {
        public ICollection<IdentityUserRole<string>> Roles { get; set; } = null!;
        public bool IsBlocked { get; set; }
    }
}
