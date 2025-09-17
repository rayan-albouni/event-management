using EventManagement.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Domain;

public class RegistrationTests
{
    [Fact]
    public void Registration_Constructor_ShouldCreateValidRegistration()
    {
        var name = "John Doe";
        var phoneNumber = "+1234567890";
        var email = "john@example.com";
        var eventId = Guid.NewGuid();

        var registration = new Registration(name, phoneNumber, email, eventId);

        registration.Name.Should().Be(name);
        registration.PhoneNumber.Should().Be(phoneNumber);
        registration.Email.Should().Be(email);
        registration.EventId.Should().Be(eventId);
        registration.Id.Should().NotBeEmpty();
        registration.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("", "+1234567890", "john@example.com")]
    [InlineData("John Doe", "", "john@example.com")]
    [InlineData("John Doe", "+1234567890", "")]
    public void Registration_Constructor_ShouldThrowArgumentException_WhenRequiredFieldsAreEmpty(
        string name, string phoneNumber, string email)
    {
        var eventId = Guid.NewGuid();

        var act = () => new Registration(name, phoneNumber, email, eventId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Registration_Constructor_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var act = () => new Registration(null!, "+1234567890", "john@example.com", Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Registration_Constructor_ShouldThrowArgumentException_WhenPhoneNumberIsNull()
    {
        var act = () => new Registration("John Doe", null!, "john@example.com", Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Registration_Constructor_ShouldThrowArgumentException_WhenEmailIsNull()
    {
        var act = () => new Registration("John Doe", "+1234567890", null!, Guid.NewGuid());
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("invalid@")]
    [InlineData("invalid.com")]
    [InlineData("@invalid")] 
    public void Registration_Constructor_ShouldThrowArgumentException_WhenEmailFormatIsInvalid(string email)
    {
        var eventId = Guid.NewGuid();

        var act = () => new Registration("John Doe", "+1234567890", email, eventId);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid email format*");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    public void Registration_Constructor_ShouldAcceptValidEmails(string email)
    {
        var eventId = Guid.NewGuid();

        var registration = new Registration("John Doe", "+1234567890", email, eventId);

        registration.Email.Should().Be(email);
    }
}
