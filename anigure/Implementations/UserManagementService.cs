using System.Security.Claims;
using anigure.Abstractions;
using anigure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace anigure.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<int> GetUsersCountAsync(string? searchString = null)
    {
        var users = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(searchString))
        {
            users = users.Where(user =>
                searchString.Equals(user.UserName) ||
                searchString.Equals(user.Email));
        }

        return users.CountAsync();
    }

    public async Task<IdentityResult> AddUserAsync(ApplicationUser user, string password, string role)
    {
        // In case of racing conditions the error will be returned by the CreateAsync method
        // This extra check is made because CreateAsync produces errors for both email and username
        // (when only the email is exposed to the GUI)

        if (user.Email != null && await _userManager.FindByEmailAsync(user.Email) != null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Email already in use!" });
        }

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        return result;
    }

    public async Task<ApplicationUser?> GetUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        return await _userManager.GetUserAsync(claimsPrincipal);
    }
}