namespace EventManagement.Domain.Enums;

public enum Permission
{
    // Event permissions
    CreateEvent = 1,
    ReadEvent = 2,
    UpdateEvent = 3,
    DeleteEvent = 4,

    // Registration permissions
    CreateRegistration = 5,
    ReadRegistration = 6,
    DeleteRegistration = 7,
    ReadEventRegistrations = 8
}
