using EventManagement.Application.DTOs;

namespace EventManagement.Application.Interfaces;

public interface IEventService
{
    Task<EventDto> CreateEventAsync(CreateEventDto createEventDto, Guid creatorId);
    Task<EventDto> UpdateEventAsync(Guid eventId, UpdateEventDto updateEventDto);
    Task DeleteEventAsync(Guid eventId);
    Task<EventDto?> GetEventByIdAsync(Guid eventId);
    Task<IEnumerable<EventDto>> GetAllEventsAsync();
    Task<IEnumerable<EventDto>> GetEventsByCreatorAsync(Guid creatorId);
}
