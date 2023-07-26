using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Api.Services.Notifications;
using Volunteer.Api.Services.Users;
using Volunteer.Infrastructure;
using Volunteer.Models.Requests;
using Volunteer.Models.User;

namespace Volunteer.Api.Controllers;

[Route("infrastructure")]
[ApiController]
public class InfrastructureController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IUserManager _customUserManager;
    private readonly UserManager<UserIdentity> _userManager;
    private readonly INotificationManager _notificationManager;
    private readonly IMapper _mapper;

    public InfrastructureController(DataContext context,
        IUserManager customUserManager,
        UserManager<UserIdentity> userManager,
        INotificationManager notificationManager,
        IMapper mapper)
    {
        _context = context;
        _customUserManager = customUserManager;
        _userManager = userManager;
        _notificationManager = notificationManager;
        _mapper = mapper;
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

    [Route("notification-send/{userId}")]
    [HttpPost]
    public async Task<IActionResult> SendNotification(Guid userId, NotificationDto dto)
    {
        Notification notification = await _notificationManager.CreateNotificationAsync(dto);
        _notificationManager.SendNotification(notification);

        return Ok();
    }
}