using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Volunteer.Api.Controllers;

/// <summary>
/// Контроллер для администраторов
/// </summary>
[Authorize("admin")]
[ApiController]
public class AdminController : ControllerBase
{
    
    
    
}