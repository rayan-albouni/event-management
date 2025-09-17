using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetActiveEventsAsync();
}
