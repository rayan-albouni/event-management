using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Domain;

public class EventTests
{
    [Fact]
    public void Event_Constructor_ShouldCreateValidEvent()
    {
        var name = "Tech Conference";
        var description = "Annual tech conference";
        var location = "Convention Center";
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(8);
        var creatorId = Guid.NewGuid();

        var eventEntity = new Event(name, description, location, startTime, endTime, creatorId);

        eventEntity.Name.Should().Be(name);
        eventEntity.Description.Should().Be(description);
        eventEntity.Location.Should().Be(location);
        eventEntity.StartTime.Should().Be(startTime);
        eventEntity.EndTime.Should().Be(endTime);
        eventEntity.CreatorId.Should().Be(creatorId);
        eventEntity.Id.Should().NotBeEmpty();
        eventEntity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        eventEntity.Registrations.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "description", "location")]
    [InlineData("name", "", "location")]
    [InlineData("name", "description", "")]
    [InlineData(null, "description", "location")]
    [InlineData("name", null, "location")]
    [InlineData("name", "description", null)]
    public void Event_Constructor_ShouldThrowArgumentException_WhenRequiredFieldsAreEmpty(
        string? name, string? description, string? location)
    {
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(8);
        var creatorId = Guid.NewGuid();

        var act = () => new Event(name, description, location, startTime, endTime, creatorId);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Event_Constructor_ShouldThrowArgumentException_WhenStartTimeIsInPast()
    {
        var startTime = DateTime.UtcNow.AddDays(-1);
        var endTime = startTime.AddHours(8);

        var act = () => new Event("name", "description", "location", startTime, endTime, Guid.NewGuid());
        act.Should().Throw<ArgumentException>()
            .WithMessage("*start time must be in the future*");
    }

    [Fact]
    public void Event_Constructor_ShouldThrowArgumentException_WhenEndTimeIsBeforeStartTime()
    {
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(-1);

        var act = () => new Event("name", "description", "location", startTime, endTime, Guid.NewGuid());
        act.Should().Throw<ArgumentException>()
            .WithMessage("*end time must be after start time*");
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateEventDetails()
    {
        var eventEntity = CreateValidEvent();
        var newName = "Updated Conference";
        var newDescription = "Updated description";
        var newLocation = "New Location";
        var newStartTime = DateTime.UtcNow.AddDays(2);
        var newEndTime = newStartTime.AddHours(6);

        eventEntity.UpdateDetails(newName, newDescription, newLocation, newStartTime, newEndTime);

        eventEntity.Name.Should().Be(newName);
        eventEntity.Description.Should().Be(newDescription);
        eventEntity.Location.Should().Be(newLocation);
        eventEntity.StartTime.Should().Be(newStartTime);
        eventEntity.EndTime.Should().Be(newEndTime);
        eventEntity.UpdatedAt.Should().NotBeNull();
        eventEntity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void IsRegistrationOpen_ShouldReturnTrue_WhenCurrentTimeIsBeforeStartTime()
    {
        var startTime = DateTime.UtcNow.AddHours(1);
        var endTime = startTime.AddHours(8);
        var eventEntity = new Event("name", "description", "location", startTime, endTime, Guid.NewGuid());

        var result = eventEntity.IsRegistrationOpen();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsRegistrationOpen_ShouldReturnFalse_WhenCurrentTimeIsAfterStartTime()
    {
        var startTime = DateTime.UtcNow.AddMilliseconds(100);
        var endTime = startTime.AddHours(8);
        var eventEntity = new Event("name", "description", "location", startTime, endTime, Guid.NewGuid());

        System.Threading.Thread.Sleep(200);
        var result = eventEntity.IsRegistrationOpen();

        result.Should().BeFalse();
    }

    [Fact]
    public void Event_ShouldHaveEmptyRegistrationsCollection()
    {
        var eventEntity = CreateValidEvent();

        eventEntity.Registrations.Should().BeEmpty();
    }

    private static Event CreateValidEvent(Guid? creatorId = null)
    {
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(8);
        return new Event(
            "Tech Conference",
            "Annual tech conference",
            "Convention Center",
            startTime,
            endTime,
            creatorId ?? Guid.NewGuid()
        );
    }
}
