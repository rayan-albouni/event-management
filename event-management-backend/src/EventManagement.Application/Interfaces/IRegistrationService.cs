using EventManagement.Application.DTOs;

namespace EventManagement.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegistrationDto> RegisterForEventAsync(Guid eventId, CreateRegistrationDto createRegistrationDto);
    Task<IEnumerable<RegistrationDto>> GetRegistrationsByEventIdAsync(Guid eventId);
    Task DeleteRegistrationAsync(Guid registrationId);
}
