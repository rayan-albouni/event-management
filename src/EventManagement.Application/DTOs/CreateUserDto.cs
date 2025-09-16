namespace EventManagement.Application.DTOs;

public record CreateUserDto(
    string Email,
    string Password,
    string Role
);
