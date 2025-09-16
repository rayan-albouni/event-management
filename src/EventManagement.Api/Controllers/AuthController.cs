using EventManagement.Application.DTOs;
using EventManagement.Application.Interfaces;
using EventManagement.Domain.Enums;
using EventManagement.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            if (!IsValidRole(createUserDto.Role))
            {
                return BadRequest(new
                {
                    message = $"Invalid role. Allowed roles: {string.Join(", ", GetAllowedRoles())}"
                });
            }

            var result = await _authService.RegisterAsync(createUserDto);
            return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
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

    [HttpGet("profile")]
    [Authorize]
    public ActionResult<object> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var email = GetCurrentUserEmail();
            var roleString = GetCurrentUserRole();

            if (!Enum.TryParse<UserRole>(roleString, out var role))
            {
                return BadRequest(new { message = "Invalid user role in token" });
            }

            var permissions = RolePermissions.GetPermissions(role);

            var profile = new
            {
                Id = userId,
                Email = email,
                Role = roleString,
                Permissions = permissions.Select(p => p.ToString()).ToArray()
            };

            return Ok(profile);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    #region Private Helper Methods

    private static bool IsValidRole(string role)
    {
        return Enum.TryParse<UserRole>(role, out _);
    }

    private static string[] GetAllowedRoles()
    {
        return Enum.GetNames<UserRole>();
    }

    #endregion
}
