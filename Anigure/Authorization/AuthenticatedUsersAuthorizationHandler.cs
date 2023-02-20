using Anigure.Data;
using Anigure.Data.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Anigure.Authorization
{
    public class AuthenticatedUsersAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, IResource>
    {
        private readonly List<string> allowedOperations = new List<string>
        {
            Constants.CreateOperationName,
            Constants.UpdateOperationName,
            Constants.DeleteOperationName
        };

        private readonly UserManager<ApplicationUser> _userManager;

        public AuthenticatedUsersAuthorizationHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            IResource resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            if (requirement.Name == Constants.ReadOperationName)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (allowedOperations.Contains(requirement.Name))
            {
                if (resource.Id == _userManager.GetUserId(context.User))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
