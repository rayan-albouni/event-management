using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetEventsByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<Event>> GetActiveEventsAsync();
    Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
