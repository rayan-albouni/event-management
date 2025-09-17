namespace EventManagement.Application.DTOs;

public record RegistrationDto(
    Guid Id,
    string Name,
    string PhoneNumber,
    string Email,
    Guid EventId,
    string EventName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
