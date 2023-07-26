using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Api.Services.Events;
using Volunteer.Api.Services.Users;
using Volunteer.Infrastructure;
using Volunteer.Models.Event;
using Volunteer.Models.Responses;
using Volunteer.Models.User;
using Volunteer.Validators;

namespace Volunteer.Api.Controllers;

/// <summary>
/// Контроллер для управления и взаимодействия событиями
/// </summary>
[Authorize]
[ApiController]
[Route("event")]
public class EventsController : ControllerBase
{
    private readonly IEventManager _eventManager;
    private readonly UserManager<UserIdentity> _userManager;
    private readonly IUserManager _customUserManager;
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly EventValidator _eventValidator;

    public EventsController(IEventManager eventManager,
        UserManager<UserIdentity> userManager,
        IUserManager customUserManager,
        DataContext context,
        IMapper mapper)
    {
        _eventManager = eventManager;
        _userManager = userManager;
        _customUserManager = customUserManager;
        _context = context;
        _mapper = mapper;
        _eventValidator = new();
    }

    /// <summary>
    /// Создать новое событие
    /// </summary>
    /// <param name="newEvent">Событие</param>
    /// <returns></returns>
    [Authorize("admin")]
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(EventCreate newEvent)
    {
        var city = _context.Cities.Find(newEvent.City);
        var eventType = _context.EventTypes.Find(newEvent.Type);

        if (city == null)
            return Ok(new ErrorResponse
            {
                Error = "city_not_exists",
                Message = "Города с таким id не существует",
                StatusCode = 404
            });

        if (eventType == null)
            return Ok(new ErrorResponse
            {
                Error = "eventType_not_exists",
                Message = "Событие не может быть этого типа",
                StatusCode = 404
            });

        var fullEvent = _mapper.Map<Event>(newEvent, options =>
        {
            options.Items.Add("city", city);
            options.Items.Add("eventType", eventType);
        });
        fullEvent.Admin = await _userManager.FindByEmailAsync(HttpContext.User.Claims
            .First(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
            .Value);

        var validationResult = await _eventValidator.ValidateAsync(fullEvent);

        if (validationResult.IsValid)
        {
            await _eventManager.CreateAsync(fullEvent);

            return Ok(fullEvent.Id);
        }

        return Ok(new ErrorResponse
        {
            Error = "not_valid",
            Message = $"Мероприятие не прошло валидацию: {string.Join(", ", validationResult.Errors)}",
            StatusCode = 400
        });
    }

    /// <summary>
    /// Редактировать событие
    /// </summary>
    /// <returns></returns>
    [Authorize("verified_volunteer")]
    [Route("edit")]
    [HttpPost]
    public IActionResult Edit()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Добавить участника к событию
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="eventId">ID события</param>
    /// <returns></returns>
    [Authorize("verified_volunteer")]
    [Route("add-user")]
    [HttpPost]
    public async Task<IActionResult> AddUser(Guid userId, Guid eventId)
    {
        UserIdentity? user = await _userManager.FindByIdAsync(userId.ToString());

    Event? @event = _eventManager.Get(eventId);

        bool isUserExists = _customUserManager.GetActiveUserEvents(user)
            .Any(x => x.Id == @event.Id);

        if (isUserExists)
        {
            return Ok(new ErrorResponse
            {
                Error = "user_exists",
                Message = "Этот пользователь уже участник",
                StatusCode = 400
            });
        }

        _eventManager.AddUser(@event, user);
        return Ok();
    }

    [Route("join/{eventId}")]
    [HttpPost]
    [Authorize("volunteer")]
    public async Task<IActionResult> JoinToEvent(Guid eventId)
    {
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        var toEvent = _eventManager.Get(eventId);

        if (toEvent == null)
            return Ok(new ErrorResponse
            {
                StatusCode = 404,
                Error = "event_not_exists",
                Message = "Событие не найдено"
            });

        if (_context.ActiveUserEvents.Any(x => x.Event.Id == toEvent.Id))
        {
            return Ok(new ErrorResponse
            {
                Error = "user_exists",
                Message = "Вы уже участник",
                StatusCode = 400
            });
        }


        _eventManager.AddUser(toEvent, user);

        return Ok();
    }

    /// <summary>
    /// Пригласить пользователя в событие
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="eventId">ID события</param>
    [Authorize("verified_volunteer")]
    [Route("invite-user")]
    [HttpPost]
    public IActionResult InviteUser(Guid userId, Guid eventId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Удалить событие, может использовать только администратор,
    /// создатель события может его отключить 
    /// </summary>
    [Authorize("admin")]
    [Route("remove")]
    [HttpDelete]
    public IActionResult Remove(Guid eventId)
    {
        Event? removeEvent = _eventManager.Get(eventId);

        if (_context.Events
            .Any(x => x.Id == removeEvent.Id && x.Name == removeEvent.Name))
        {
            _eventManager.Remove(removeEvent);

            return Ok();
        }

        return Ok(new ErrorResponse
        {
            Error = "not_found",
            Message = "Мероприятие не существует",
            StatusCode = 404
        });
    }

    /// <summary>
    /// Получить событие
    /// </summary>
    /// <param name="eventId">ID нужного события</param>
    [Route("get")]
    [HttpGet]
    public IActionResult Get(Guid eventId)
    {
        return Ok(_eventManager.Get(eventId));
    }

    /// <summary>
    /// Получить список события с фильтрацией по городу и типу
    /// </summary>
    /// <param name="filterCity">ID города</param>
    /// <param name="filterType">ID типа</param>
    [Route("get/list")]
    [HttpGet]
    public IActionResult GetList(Guid filterCity, Guid filterType)
    {
        City city = _context.Cities.Find(filterCity);
        EventType type = _context.EventTypes.Find(filterType);

        List<Event> events = _eventManager.GetFilter(city, type);
        List<EventDto> eventDtos = new();

        foreach (Event eEvent in events)
            eventDtos.Add(_mapper.Map<EventDto>(eEvent));

        return Ok(eventDtos);
    }

    [Route("get/list/{take}")]
    [HttpGet]
    public IActionResult GetList(int take)
    {
        if (take <= -1)
        {
            return Ok(new ErrorResponse
            {
                Error = "take_error",
                Message = "Количество не может быть отрицательным"
            });
        }

        List<Event> events = _eventManager.GetList(take);

        return Ok(events);
    }

    [Route("search")]
    [HttpGet]
    public IActionResult Search(string search)
    {
        throw new NotImplementedException();
    }
}