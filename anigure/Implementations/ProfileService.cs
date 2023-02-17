using anigure.Abstractions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;

namespace anigure.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUserManagementService _userManagementService;

    public ProfileService(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = context.Subject.Claims
            .Where(claim => claim.Type is JwtClaimTypes.Email or JwtClaimTypes.Role)
            .ToList();

        context.IssuedClaims.AddRange(claims);

        return Task.CompletedTask;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManagementService.GetUserAsync(context.Subject);
        context.IsActive = user is not null;
    }
}