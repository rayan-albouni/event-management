using EventManagement.Domain.Enums;

namespace EventManagement.Domain.Entities;

public class User : BaseEntity
{
    public string? Email { get; private set; }
    public string? PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<Event> _createdEvents = [];
    public IReadOnlyCollection<Event> CreatedEvents => _createdEvents.AsReadOnly();

    private User() { } // For EF Core

    public User(string? email, string? passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }
}
