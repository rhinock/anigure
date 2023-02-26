using Anigure.Data;

namespace Anigure.Models.Users
{
    public class UserIndexViewModel
    {
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public Dictionary<string, string?> Roles { get; set; } = new Dictionary<string, string?>();
    }
}
