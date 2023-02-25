using Anigure.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Anigure.Data
{
    public static class SeedData
    {
        private static readonly string userName = "admin@anigure.com";
        private static readonly string password = "P@ssw0rd";

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            await using var context = new ApplicationDbContext(options);

            var adminId = await EnsureUser(serviceProvider);
            await EnsureAdministratorsRole(serviceProvider, adminId);
            await EnsureRole(serviceProvider, Roles.UsersRole);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName
                };

                await userManager.CreateAsync(user, password);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureAdministratorsRole(IServiceProvider serviceProvider, string uid)
        {
            var identityResult = await EnsureRole(serviceProvider, Roles.AdministratorsRole);

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The user not found");
            }

            identityResult = await userManager.AddToRoleAsync(user, Roles.AdministratorsRole);

            return identityResult;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string role)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var identityResult = new IdentityResult();

            if (!await roleManager.RoleExistsAsync(role))
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(role));

                if (!identityResult.Succeeded)
                {
                    throw new Exception("Role was not created");
                }
            }

            return identityResult;
        }
    }
}
