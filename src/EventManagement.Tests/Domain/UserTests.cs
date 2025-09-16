using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Domain;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldCreateValidUser()
    {
        var email = "test@example.com";
        var passwordHash = "hashedPassword";
        var role = UserRole.EventCreator;

        var user = new User(email, passwordHash, role);

        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(role);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.CreatedEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "passwordHash")]
    [InlineData("email", "")]
    [InlineData(null, "passwordHash")]
    [InlineData("email", null)]
    public void User_Constructor_ShouldThrowArgumentException_WhenRequiredFieldsAreEmpty(
        string? email, string? passwordHash)
    {
        var act = () => new User(email, passwordHash, UserRole.EventCreator);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void User_ShouldHaveCorrectDefaultValues()
    {
        var user = CreateValidUser();

        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().BeNull();
        user.CreatedEvents.Should().BeEmpty();
    }

    [Theory]
    [InlineData(UserRole.EventCreator)]
    [InlineData(UserRole.EventParticipant)]
    public void User_Constructor_ShouldAcceptDifferentRoles(UserRole role)
    {
        var user = new User("test@example.com", "passwordHash", role);

        user.Role.Should().Be(role);
    }

    private static User CreateValidUser()
    {
        return new User("test@example.com", "hashedPassword", UserRole.EventCreator);
    }
}
