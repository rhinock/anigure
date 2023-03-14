using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
