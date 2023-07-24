using Microsoft.AspNetCore.Mvc;
using Volunteer.Infrastructure;

namespace Volunteer.Api.Controllers;

[Route("infrastructure/")]
[ApiController]
public class InfrastructureController : ControllerBase
{
    private readonly DataContext _context;

    public InfrastructureController(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все города в которых может проходить событие
    /// </summary>
    [Route("get/cities")]
    [HttpGet]
    public IActionResult GetCities()
    {
        return Ok(_context.Cities.ToList());
    }

    /// <summary>
    /// Получить все типы событий
    /// </summary>
    [Route("get/eventTypes")]
    [HttpGet]
    public IActionResult GetEventTypes()
    {
        return Ok(_context.EventTypes.ToList());
    }
}