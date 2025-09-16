using EventManagement.Domain.Entities;

namespace EventManagement.Domain.Interfaces;

public interface IRegistrationRepository : IRepository<Registration>
{
    Task<IEnumerable<Registration>> GetRegistrationsByEventIdAsync(Guid eventId);
    Task<Registration?> GetRegistrationByEmailAndEventIdAsync(string email, Guid eventId);
    Task<bool> IsEmailRegisteredForEventAsync(string email, Guid eventId);
}
