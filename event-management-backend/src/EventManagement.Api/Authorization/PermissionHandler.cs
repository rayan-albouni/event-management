using EventManagement.Domain.Enums;
using EventManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventManagement.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission Permission { get; }

    public PermissionRequirement(Permission permission)
    {
        Permission = permission;
    }
}

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(roleClaim) || !Enum.TryParse<UserRole>(roleClaim, out var userRole))
        {
            return Task.CompletedTask;
        }

        if (RolePermissions.HasPermission(userRole, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public static class PolicyNames
{
    public const string CreateEvent = "CreateEvent";
    public const string ReadEvent = "ReadEvent";
    public const string UpdateEvent = "UpdateEvent";
    public const string DeleteEvent = "DeleteEvent";
    public const string CreateRegistration = "CreateRegistration";
    public const string ReadRegistration = "ReadRegistration";
    public const string DeleteRegistration = "DeleteRegistration";
    public const string ReadEventRegistrations = "ReadEventRegistrations";
}
