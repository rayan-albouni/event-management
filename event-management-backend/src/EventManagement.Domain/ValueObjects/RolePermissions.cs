using EventManagement.Domain.Enums;

namespace EventManagement.Domain.ValueObjects;

public static class RolePermissions
{
    public static readonly Dictionary<UserRole, HashSet<Permission>> Permissions = new()
    {
        {
            UserRole.EventCreator, new HashSet<Permission>
            {
                Permission.CreateEvent,
                Permission.ReadEvent,
                Permission.UpdateEvent,
                Permission.DeleteEvent,
                Permission.ReadEventRegistrations,
                Permission.DeleteRegistration,
                Permission.CreateRegistration,
                Permission.ReadRegistration
            }
        },
        {
            UserRole.EventParticipant, new HashSet<Permission>
            {
                Permission.ReadEvent,
                Permission.CreateRegistration,
                Permission.ReadRegistration
            }
        }
    };

    public static bool HasPermission(UserRole role, Permission permission)
    {
        return Permissions.ContainsKey(role) && Permissions[role].Contains(permission);
    }

    public static HashSet<Permission> GetPermissions(UserRole role)
    {
        return Permissions.ContainsKey(role) ? Permissions[role] : new HashSet<Permission>();
    }
}
