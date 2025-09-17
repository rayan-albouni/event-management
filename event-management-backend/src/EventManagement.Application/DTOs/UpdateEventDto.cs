namespace EventManagement.Application.DTOs;

public record UpdateEventDto(
    string Name,
    string Description,
    string Location,
    DateTime StartTime,
    DateTime EndTime
);
