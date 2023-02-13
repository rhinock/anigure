using anigure.Abstractions;
using anigure.Implementations;

namespace anigure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddScoped<IUserManagementService, UserManagementService>();
    }
}