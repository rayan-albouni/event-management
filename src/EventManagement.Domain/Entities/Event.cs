namespace EventManagement.Domain.Entities;

public class Event : BaseEntity
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public Guid CreatorId { get; private set; }
    public User? Creator { get; set; }

    private readonly List<Registration> _registrations = [];
    public IReadOnlyCollection<Registration> Registrations => _registrations.AsReadOnly();

    private Event() { } // For EF Core

    public Event(string? name, string? description, string? location, DateTime startTime, DateTime endTime, Guid creatorId)
    {
        ValidateEventData(name, description, location, startTime, endTime);

        Name = name;
        Description = description;
        Location = location;
        StartTime = startTime;
        EndTime = endTime;
        CreatorId = creatorId;
    }

    public void UpdateDetails(string name, string description, string location, DateTime startTime, DateTime endTime)
    {
        ValidateEventData(name, description, location, startTime, endTime);

        Name = name;
        Description = description;
        Location = location;
        StartTime = startTime;
        EndTime = endTime;
        SetUpdatedAt();
    }

    public bool IsRegistrationOpen() => DateTime.UtcNow < StartTime;

    private static void ValidateEventData(string? name, string? description, string? location, DateTime startTime, DateTime endTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Event description cannot be empty", nameof(description));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Event location cannot be empty", nameof(location));

        if (startTime <= DateTime.UtcNow)
            throw new ArgumentException("Event start time must be in the future", nameof(startTime));

        if (endTime <= startTime)
            throw new ArgumentException("Event end time must be after start time", nameof(endTime));
    }
}
