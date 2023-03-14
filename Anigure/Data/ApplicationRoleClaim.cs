using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
