namespace EventManagement.Application.DTOs;

public record CreateRegistrationDto(
    string Name,
    string PhoneNumber,
    string Email
);
