using Microsoft.AspNetCore.Authorization;

namespace Showaspnetcore.AuthorizationPolicies
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; set; }
    }
}