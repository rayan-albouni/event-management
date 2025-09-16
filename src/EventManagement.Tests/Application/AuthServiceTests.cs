using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using EventManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace EventManagement.Tests.Application;

public class AuthServiceTests
{
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IPasswordHasher _mockPasswordHasher;
    private readonly IConfiguration _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockPasswordHasher = Substitute.For<IPasswordHasher>();
        _mockConfiguration = Substitute.For<IConfiguration>();

        var jwtSection = Substitute.For<IConfigurationSection>();
        _mockConfiguration.GetSection("JwtSettings").Returns(jwtSection);
        jwtSection["SecretKey"].Returns("test-secret-key-that-is-long-enough-for-jwt-signing-purposes");
        jwtSection["Issuer"].Returns("TestIssuer");
        jwtSection["ExpirationHours"].Returns("24");

        _authService = new AuthService(_mockUnitOfWork, _mockPasswordHasher, _mockConfiguration);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var email = "test@example.com";
        var password = "password123";
        var passwordHash = "hashedPassword";
        var user = new User(email, passwordHash, UserRole.EventCreator);

        var loginDto = new LoginDto(email, password);

        _mockUnitOfWork.Users.GetByEmailAsync(email).Returns(user);
        _mockPasswordHasher.VerifyPassword(password, passwordHash).Returns(true);

        var result = await _authService.LoginAsync(loginDto);

        result.Should().NotBeNull();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(email);
        result.Token.Should().NotBeNullOrEmpty();
        await _mockUnitOfWork.Users.Received(1).GetByEmailAsync(email);
        _mockPasswordHasher.Received(1).VerifyPassword(password, passwordHash);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
    {
        var loginDto = new LoginDto("nonexistent@example.com", "password123");
        _mockUnitOfWork.Users.GetByEmailAsync(loginDto.Email).Returns((User?)null);

        var act = async () => await _authService.LoginAsync(loginDto);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsInactive()
    {
        var email = "test@example.com";
        var user = new User(email, "hashedPassword", UserRole.EventCreator);
        
        var isActiveField = typeof(User).GetField("_isActive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isActiveField != null)
        {
            isActiveField.SetValue(user, false);
        }

        var loginDto = new LoginDto(email, "password123");
        _mockUnitOfWork.Users.GetByEmailAsync(email).Returns(user);

        var act = async () => await _authService.LoginAsync(loginDto);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
    {
        var email = "test@example.com";
        var password = "wrongpassword";
        var user = new User(email, "hashedPassword", UserRole.EventCreator);

        var loginDto = new LoginDto(email, password);
        _mockUnitOfWork.Users.GetByEmailAsync(email).Returns(user);
        _mockPasswordHasher.VerifyPassword(password, user.PasswordHash!).Returns(false);

        var act = async () => await _authService.LoginAsync(loginDto);
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnUserDto_WhenRegistrationIsValid()
    {
        var createUserDto = new CreateUserDto("test@example.com", "password123", "EventCreator");
        var hashedPassword = "hashedPassword123";

        _mockUnitOfWork.Users.EmailExistsAsync(createUserDto.Email).Returns(false);
        _mockPasswordHasher.HashPassword(createUserDto.Password).Returns(hashedPassword);

        var result = await _authService.RegisterAsync(createUserDto);

        result.Should().NotBeNull();
        result.Email.Should().Be(createUserDto.Email);
        result.Role.Should().Be(createUserDto.Role);
        result.IsActive.Should().BeTrue();

        await _mockUnitOfWork.Users.Received(1).EmailExistsAsync(createUserDto.Email);
        _mockPasswordHasher.Received(1).HashPassword(createUserDto.Password);
        await _mockUnitOfWork.Users.Received(1).AddAsync(Arg.Any<User>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
    {
        var createUserDto = new CreateUserDto("existing@example.com", "password123", "EventCreator");
        _mockUnitOfWork.Users.EmailExistsAsync(createUserDto.Email).Returns(true);

        var act = async () => await _authService.RegisterAsync(createUserDto);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowArgumentException_WhenRoleIsInvalid()
    {
        var createUserDto = new CreateUserDto("test@example.com", "password123", "InvalidRole");
        _mockUnitOfWork.Users.EmailExistsAsync(createUserDto.Email).Returns(false);

        var act = async () => await _authService.RegisterAsync(createUserDto);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid user role");
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        var userDto = new UserDto(
            Guid.NewGuid(),
            "test@example.com",
            "EventCreator",
            true,
            DateTime.UtcNow,
            null
        );

        var token = _authService.GenerateJwtToken(userDto);

        token.Should().NotBeNullOrEmpty();
        var parts = token.Split('.');
        parts.Should().HaveCount(3);
    }
}
