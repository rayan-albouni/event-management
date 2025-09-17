using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Domain.Entities;
using EventManagement.Domain.Interfaces;

namespace EventManagement.Application.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RegistrationDto> RegisterForEventAsync(Guid eventId, CreateRegistrationDto createRegistrationDto)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found");

        if (!eventEntity.IsRegistrationOpen())
            throw new InvalidOperationException("Registration is closed for this event");

        if (await _unitOfWork.Registrations.IsEmailRegisteredForEventAsync(
            createRegistrationDto.Email, eventId))
            throw new InvalidOperationException("Email is already registered for this event");

        var registration = new Registration(
            createRegistrationDto.Name,
            createRegistrationDto.PhoneNumber,
            createRegistrationDto.Email,
            eventId
        );

        await _unitOfWork.Registrations.AddAsync(registration);
        await _unitOfWork.SaveChangesAsync();

        return await MapToRegistrationDtoAsync(registration);
    }

    public async Task<IEnumerable<RegistrationDto>> GetRegistrationsByEventIdAsync(Guid eventId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found");

        var registrations = await _unitOfWork.Registrations.GetRegistrationsByEventIdAsync(eventId);
        var registrationDtos = new List<RegistrationDto>();

        foreach (var registration in registrations)
        {
            registrationDtos.Add(await MapToRegistrationDtoAsync(registration));
        }

        return registrationDtos;
    }

    public async Task DeleteRegistrationAsync(Guid registrationId)
    {
        var registration = await _unitOfWork.Registrations.GetByIdAsync(registrationId);
        if (registration == null)
            throw new ArgumentException("Registration not found");

        var eventEntity = await _unitOfWork.Events.GetByIdAsync(registration.EventId);
        if (eventEntity == null)
            throw new ArgumentException("Event not found");

        await _unitOfWork.Registrations.DeleteAsync(registrationId);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<RegistrationDto> MapToRegistrationDtoAsync(Registration registration)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(registration.EventId);

        return new RegistrationDto(
            registration.Id,
            registration.Name ?? string.Empty,
            registration.PhoneNumber ?? string.Empty,
            registration.Email ?? string.Empty,
            registration.EventId,
            eventEntity?.Name ?? "Unknown Event",
            registration.CreatedAt,
            registration.UpdatedAt
        );
    }
}
