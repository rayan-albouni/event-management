namespace EventManagement.Domain.Entities;

public class Registration : BaseEntity
{
    public string? Name { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public Guid EventId { get; private set; }
    public Event? Event { get; private set; }

    private Registration() { } // For EF Core

    public Registration(string name, string phoneNumber, string email, Guid eventId)
    {
        ValidateRegistrationData(name, phoneNumber, email);

        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        EventId = eventId;
    }

    private static void ValidateRegistrationData(string name, string phoneNumber, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Registration name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format", nameof(email));
    }
}
