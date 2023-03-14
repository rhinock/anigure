using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
