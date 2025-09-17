using EventManagement.Application.DTOs;

namespace EventManagement.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
    string GenerateJwtToken(UserDto user);
}
