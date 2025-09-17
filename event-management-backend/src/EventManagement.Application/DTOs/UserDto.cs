namespace EventManagement.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
