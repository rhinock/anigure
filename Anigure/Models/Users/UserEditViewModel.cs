using Anigure.Data;

namespace Anigure.Models.Users
{
    public class UserEditViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public Dictionary<string, string?> Roles { get; set; } = new Dictionary<string, string?>();
    }
}
