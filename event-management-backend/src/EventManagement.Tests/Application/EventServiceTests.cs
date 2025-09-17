using EventManagement.Application.DTOs;
using EventManagement.Application.Services;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using EventManagement.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace EventManagement.Tests.Application;

public class EventServiceTests
{
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _eventService = new EventService(_mockUnitOfWork);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldReturnEventDto_WhenEventIsValid()
    {
        var creatorId = Guid.NewGuid();
        var creator = new User("creator@example.com", "passwordHash", UserRole.EventCreator);
        var createEventDto = new CreateEventDto(
            "Tech Conference",
            "Annual tech conference",
            "Convention Center",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(8)
        );

        _mockUnitOfWork.Users.GetByIdAsync(creatorId).Returns(creator);

        var result = await _eventService.CreateEventAsync(createEventDto, creatorId);

        result.Should().NotBeNull();
        result.Name.Should().Be(createEventDto.Name);
        result.Description.Should().Be(createEventDto.Description);
        result.Location.Should().Be(createEventDto.Location);
        result.StartTime.Should().Be(createEventDto.StartTime);
        result.EndTime.Should().Be(createEventDto.EndTime);

        await _mockUnitOfWork.Users.Received(1).GetByIdAsync(creatorId);
        await _mockUnitOfWork.Events.Received(1).AddAsync(Arg.Any<Event>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowArgumentException_WhenCreatorNotFound()
    {
        var creatorId = Guid.NewGuid();
        var createEventDto = new CreateEventDto(
            "Tech Conference",
            "Annual tech conference",
            "Convention Center",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(8)
        );

        _mockUnitOfWork.Users.GetByIdAsync(creatorId).Returns((User?)null);

        var act = async () => await _eventService.CreateEventAsync(createEventDto, creatorId);
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Creator not found");
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateEvent_EvenWhenUserIsEventParticipant()
    {
        var creatorId = Guid.NewGuid();
        var creator = new User("participant@example.com", "passwordHash", UserRole.EventParticipant);
        var createEventDto = new CreateEventDto(
            "Tech Conference",
            "Annual tech conference",
            "Convention Center",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(8)
        );

        _mockUnitOfWork.Users.GetByIdAsync(creatorId).Returns(creator);

        var result = await _eventService.CreateEventAsync(createEventDto, creatorId);

        result.Should().NotBeNull();
        result.Name.Should().Be(createEventDto.Name);
        await _mockUnitOfWork.Users.Received(1).GetByIdAsync(creatorId);
        await _mockUnitOfWork.Events.Received(1).AddAsync(Arg.Any<Event>());
        await _mockUnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnEventDtos()
    {
        var events = new List<Event>
        {
            CreateValidEvent("Event 1"),
            CreateValidEvent("Event 2")
        };

        _mockUnitOfWork.Events.GetActiveEventsAsync().Returns(events);
        _mockUnitOfWork.Users.GetByIdAsync(Arg.Any<Guid>()).Returns(new User("creator@example.com", "hash", UserRole.EventCreator));

        var result = await _eventService.GetAllEventsAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Event 1");
        result.Last().Name.Should().Be("Event 2");

        await _mockUnitOfWork.Events.Received(1).GetActiveEventsAsync();
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldReturnEventDto_WhenEventExists()
    {
        var eventId = Guid.NewGuid();
        var eventEntity = CreateValidEvent("Test Event");
        
        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns(eventEntity);
        _mockUnitOfWork.Users.GetByIdAsync(eventEntity.CreatorId).Returns(new User("creator@example.com", "hash", UserRole.EventCreator));

        var result = await _eventService.GetEventByIdAsync(eventId);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Event");

        await _mockUnitOfWork.Events.Received(1).GetByIdAsync(eventId);
    }

    [Fact]
    public async Task GetEventByIdAsync_ShouldReturnNull_WhenEventNotFound()
    {
        var eventId = Guid.NewGuid();
        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns((Event?)null);

        var result = await _eventService.GetEventByIdAsync(eventId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldReturnUpdatedEventDto_WhenEventIsValid()
    {
        var eventId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();
        var eventEntity = CreateValidEvent("Original Event", creatorId);
        var updateEventDto = new UpdateEventDto(
            "Updated Event",
            "Updated description",
            "Updated location",
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(2).AddHours(6)
        );

        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns(eventEntity);
        _mockUnitOfWork.Users.GetByIdAsync(creatorId).Returns(new User("creator@example.com", "hash", UserRole.EventCreator));

        var result = await _eventService.UpdateEventAsync(eventId, updateEventDto);

        result.Should().NotBeNull();
        result.Name.Should().Be(updateEventDto.Name);
        result.Description.Should().Be(updateEventDto.Description);

        await _mockUnitOfWork.Events.Received(1).GetByIdAsync(eventId);
        await _mockUnitOfWork.Events.Received(1).UpdateAsync(eventEntity);
        await _mockUnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public void UpdateEventAsync_ShouldThrowArgumentException_WhenEventNotFound()
    {
        var eventId = Guid.NewGuid();
        var updateEventDto = new UpdateEventDto(
            "Updated Event",
            "Updated description",
            "Updated location",
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(2).AddHours(6)
        );

        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns((Event?)null);

        var act = async () => await _eventService.UpdateEventAsync(eventId, updateEventDto);
        act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Event not found");
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldDeleteEvent()
    {
        var eventId = Guid.NewGuid();
        var eventEntity = CreateValidEvent("Test Event");

        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns(eventEntity);

        await _eventService.DeleteEventAsync(eventId);

        await _mockUnitOfWork.Events.Received(1).GetByIdAsync(eventId);
        await _mockUnitOfWork.Events.Received(1).DeleteAsync(eventId);
        await _mockUnitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public void DeleteEventAsync_ShouldThrowArgumentException_WhenEventNotFound()
    {
        var eventId = Guid.NewGuid();
        _mockUnitOfWork.Events.GetByIdAsync(eventId).Returns((Event?)null);

        var act = async () => await _eventService.DeleteEventAsync(eventId);
        act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Event not found");
    }

    private static Event CreateValidEvent(string name = "Test Event", Guid? creatorId = null)
    {
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(8);
        return new Event(
            name,
            "Test description",
            "Test location",
            startTime,
            endTime,
            creatorId ?? Guid.NewGuid()
        );
    }
}
