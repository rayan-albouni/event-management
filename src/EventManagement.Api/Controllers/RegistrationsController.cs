using EventManagement.Api.Authorization;
using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Controllers;

[ApiController]
[Route("api/v1/events/{eventId}/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpGet]
    [Authorize(Policy = PolicyNames.ReadEventRegistrations)]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetRegistrationsByEventId(Guid eventId)
    {
        try
        {
            var registrations = await _registrationService.GetRegistrationsByEventIdAsync(eventId);
            return Ok(registrations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = PolicyNames.CreateRegistration)]
    public async Task<ActionResult<RegistrationDto>> RegisterForEvent(Guid eventId, [FromBody] CreateRegistrationDto createRegistrationDto)
    {
        try
        {
            var registration = await _registrationService.RegisterForEventAsync(eventId, createRegistrationDto);
            return CreatedAtAction(nameof(RegisterForEvent), new { id = registration.Id }, registration);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   

    [HttpDelete("{registrationId}")]
    [Authorize(Policy = PolicyNames.DeleteRegistration)]
    public async Task<ActionResult> DeleteRegistration(Guid registrationId)
    {
        try
        {
            await _registrationService.DeleteRegistrationAsync(registrationId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
