using Microsoft.AspNetCore.Identity;

namespace Anigure.Models.Users
{
    public class UserViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
