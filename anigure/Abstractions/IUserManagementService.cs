using anigure.Models;
using Microsoft.AspNetCore.Identity;

namespace anigure.Abstractions;

public interface IUserManagementService
{
    Task<int> GetUsersCountAsync(string? searchString = null);
    Task<IdentityResult> AddUserAsync(ApplicationUser user, string password, string role);
}