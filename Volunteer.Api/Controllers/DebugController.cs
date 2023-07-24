#if DEBUG
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Models.User;

namespace Volunteer.Api.Controllers;

[Route("debug")]
[ApiController]
public class DebugController : Controller
{
    private readonly UserManager<UserIdentity> _userManager;

    public DebugController(UserManager<UserIdentity> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Route("get-roles")]
    public async Task<IActionResult> GetRoles()
    {
        var user = await _userManager.FindByEmailAsync(HttpContext.User.Claims
            .First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value);

        return Ok(new List<string>(await _userManager.GetRolesAsync(user)));
    }

    [HttpGet]
    [Route("get-claims")]
    public IActionResult GetClaims()
    {
        var claims = HttpContext.User.Claims;

        List<string> strings = new List<string>();
        foreach (Claim claim in claims)
            strings.Add($"{claim.Type} : {claim.Value}");

        return Ok(strings);
    }
}
#endif