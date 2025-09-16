using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using EventManagement.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text;

namespace EventManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationHours;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;

        var jwtSettings = configuration.GetSection("JwtSettings");
        _jwtSecret = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JWT SecretKey is not configured");
        _jwtIssuer = jwtSettings["Issuer"] ?? throw new ArgumentNullException("JWT Issuer is not configured");
        _jwtAudience = jwtSettings["Audience"] ?? throw new ArgumentNullException("JWT Audience is not configured");
        _jwtExpirationHours = int.TryParse(jwtSettings["ExpirationHours"], out var hours) ? hours : 24;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (string.IsNullOrEmpty(user.PasswordHash) || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        var userDto = MapToUserDto(user);
        var token = GenerateJwtToken(userDto);

        return new AuthResponseDto(token, userDto);
    }

    public async Task<UserDto> RegisterAsync(CreateUserDto createUserDto)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(createUserDto.Email))
            throw new InvalidOperationException("Email already exists");

        if (!Enum.TryParse<UserRole>(createUserDto.Role, out var role))
            throw new ArgumentException("Invalid user role");

        var passwordHash = _passwordHasher.HashPassword(createUserDto.Password);
        var user = new User(createUserDto.Email, passwordHash, role);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToUserDto(user);
    }

    public string GenerateJwtToken(UserDto user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var claimsIdentity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        });

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(_jwtExpirationHours),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.Role.ToString(),
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
