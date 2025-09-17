using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Interfaces;

namespace EventManagement.Application.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, Guid creatorId)
    {
        var creator = await _unitOfWork.Users.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Creator not found");

        var eventEntity = new Event(
            createEventDto.Name,
            createEventDto.Description,
            createEventDto.Location,
            createEventDto.StartTime,
            createEventDto.EndTime,
            creatorId
        );

        await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return await MapToEventDtoAsync(eventEntity);
    }

    public async Task<EventDto> UpdateEventAsync(Guid eventId, UpdateEventDto updateEventDto)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found");

        eventEntity.UpdateDetails(
            updateEventDto.Name,
            updateEventDto.Description,
            updateEventDto.Location,
            updateEventDto.StartTime,
            updateEventDto.EndTime
        );

        await _unitOfWork.Events.UpdateAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return await MapToEventDtoAsync(eventEntity);
    }

    public async Task DeleteEventAsync(Guid eventId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found");

        await _unitOfWork.Events.DeleteAsync(eventId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<EventDto?> GetEventByIdAsync(Guid eventId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        return eventEntity == null ? null : await MapToEventDtoAsync(eventEntity);
    }

    public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
    {
        var events = await _unitOfWork.Events.GetActiveEventsAsync();
        var eventDtos = new List<EventDto>();

        foreach (var eventEntity in events)
        {
            eventDtos.Add(await MapToEventDtoAsync(eventEntity));
        }

        return eventDtos;
    }

    private async Task<EventDto> MapToEventDtoAsync(Event eventEntity)
    {
        var registrations = eventEntity.Registrations ?? await _unitOfWork.Registrations.GetRegistrationsByEventIdAsync(eventEntity.Id);

        return new EventDto(
            eventEntity.Id,
            eventEntity.Name ?? string.Empty,
            eventEntity.Description ?? string.Empty,
            eventEntity.Location ?? string.Empty,
            eventEntity.StartTime,
            eventEntity.EndTime,
            eventEntity.CreatorId,
            registrations.Count(),
            eventEntity.CreatedAt,
            eventEntity.UpdatedAt
        );
    }
}
