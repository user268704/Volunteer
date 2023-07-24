using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public AdminController(UserManager<UserIdentity> userManager)
    {
        _userManager = userManager;
    }


    [AllowAnonymous]
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

    [Authorize("admin")]
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