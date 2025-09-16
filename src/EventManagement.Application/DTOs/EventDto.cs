namespace EventManagement.Application.DTOs;

public record EventDto(
    Guid Id,
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime,
    Guid CreatorId,
    int RegistrationCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
