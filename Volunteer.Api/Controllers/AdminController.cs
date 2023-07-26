using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Api.Services.Events;
using Volunteer.Infrastructure;
using Volunteer.Models.Responses;
using Volunteer.Models.User;

namespace Volunteer.Api.Controllers;

/// <summary>
/// Контроллер для администраторов
/// </summary>
[Authorize("admin")]
[Route("admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly UserManager<UserIdentity> _userManager;
    private readonly IEventManager _eventManager;
    private readonly DataContext _context;

    public AdminController(UserManager<UserIdentity> userManager,
        IEventManager eventManager,
        DataContext context)
    {
        _userManager = userManager;
        _eventManager = eventManager;
        _context = context;
    }

    [Route("make-admin")]
    [HttpPost]
    public async Task<IActionResult> MakeAdmin([FromBody] Guid uid)
    {
        var user = await _userManager.FindByIdAsync(uid.ToString());
        if (user == null)
            return Ok(new ErrorResponse
            {
                Error = "user_not_found",
                Message = "Такого пользователя не существует",
                StatusCode = 404
            });

        var toRoleResult = await _userManager.AddToRoleAsync(user, "admin");

        if (toRoleResult.Succeeded)
        {
            return Ok();
        }

        return Ok(new ErrorResponse
        {
            Error = "add_to_role_error",
            Message = string.Join(", ", toRoleResult.Errors.Select(x => x.Description))
        });
    }

    [Route("verified")]
    [HttpPost]
    public IActionResult VerifiedEvent(Guid eventId)
    {
        var notVerifiedEvent = _eventManager.Get(eventId);
        if (notVerifiedEvent == null)
            return Ok(new ErrorResponse
            {
                Error = "event_not_exists",
                Message = "Событие с таким id не существует",
                StatusCode = 404
            });

        notVerifiedEvent.IsConfirmed = true;
        _context.SaveChanges();

        return Ok();
    }
    
    [Route("verified-volunteer")]
    [HttpPost]
    public async Task<IActionResult> VerifiedVolunteer([FromBody] Guid uid)
    {
        var user = await _userManager.FindByIdAsync(uid.ToString());
        if (user == null)
            return Ok(new ErrorResponse
            {
                Error = "user_not_found",
                Message = "Такого пользователя не существует",
                StatusCode = 404
            });

        var toRoleResult = await _userManager.AddToRoleAsync(user, "verified_volunteer");

        if (toRoleResult.Succeeded)
        {
            return Ok();
        }

        return Ok(new ErrorResponse
        {
            Error = "add_to_role_error",
            Message = string.Join(", ", toRoleResult.Errors.Select(x => x.Description))
        });
    }
}