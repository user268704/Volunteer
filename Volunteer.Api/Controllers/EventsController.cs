using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Volunteer.Api.Filters;
using Volunteer.Api.Services.Events;
using Volunteer.Infrastructure;
using Volunteer.Models.Event;
using Volunteer.Models.Responses;
using Volunteer.Models.User;
using Volunteer.Validators;

namespace Volunteer.Api.Controllers;

[NullFilter]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("event")]
public class EventsController : ControllerBase
{
    private readonly IEventManager _eventManager;
    private readonly UserManager<UserIdentity> _userManager;
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly EventValidator _eventValidator;

    public EventsController(IEventManager eventManager,
        UserManager<UserIdentity> userManager,
        DataContext context,
        IMapper mapper)
    {
        _eventManager = eventManager;
        _userManager = userManager;
        _context = context;
        _mapper = mapper;
        _eventValidator = new();
    }
    
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> Create(EventDto newEvent)
    {
        var fullEvent = _mapper.Map<Event>(newEvent);
        fullEvent.Admin = await _userManager.FindByIdAsync(newEvent.Admin.ToString());
        fullEvent.Participants = new List<UserIdentity>();
        
        foreach (Guid participant in newEvent.Participants.DistinctBy(x => x))
            fullEvent.Participants.Add(await _userManager.FindByIdAsync(participant.ToString()));

        var validationResult = await _eventValidator.ValidateAsync(fullEvent);
        
        if (validationResult.IsValid)
        {
            await _eventManager.CreateAsync(fullEvent);
            
            return Ok();
        }
        
        return Ok(new ErrorResponse
        {
            Error = "not_valid",
            Message = $"Мероприятие не прошло валидацию: {string.Join(", ", validationResult.Errors)}"
        });
    }

    [Route("edit")]
    [HttpPost]
    public IActionResult Edit()
    {
        throw new NotImplementedException();
        
    }

    [Route("add-user")]
    [HttpPost]
    public async Task<IActionResult> AddUser(Guid uid, Guid eid)
    {
        UserIdentity? user = await _userManager.FindByIdAsync(uid.ToString());
        Event? @event = _eventManager.Get(eid);
        
        bool isUserExists = @event.Participants
            .Any(participant => participant.Id == user.Id && participant.UserName == user.UserName);

        if (isUserExists)
        {
            return Ok(new ErrorResponse
            {
                Error = "user_exists",
                Message = "Этот пользователь уже участник"
            });
        }
        
        _eventManager.AddUser(@event, user);
        return Ok();
    }

    [Route("invite-user")]
    [HttpPost]
    public IActionResult InviteUser(Guid uid, Guid eid)
    {
        throw new NotImplementedException();
        
    }

    [Route("remove")]
    [HttpDelete]
    public IActionResult Remove(Guid eid)
    {
        Event? removeEvent = _eventManager.Get(eid);
        
        if (_context.Events
            .Any(x => x.Id == removeEvent.Id && x.Name == removeEvent.Name))
        {
            _eventManager.Remove(removeEvent);

            return Ok();
        }

        return Ok(new ErrorResponse
        {
            Error = "not_found",
            Message = "Мероприятие не существует"
        });
    }

    [Route("get")]
    [HttpGet]
    public IActionResult Get(Guid eid)
    {
        return Ok(_eventManager.Get(eid));
    }

    [Route("get/list")]
    [HttpGet]
    public IActionResult GetList(Guid fc, Guid ft)
    {
        City city = _context.Cities.Find(fc);
        EventType type = _context.EventTypes.Find(ft);

        List<Event> events = _eventManager.GetFilter(city, type);
        List<EventDto> eventDtos = new();
        
        foreach (Event eEvent in events) 
            eventDtos.Add(_mapper.Map<EventDto>(eEvent));

        return Ok(eventDtos);
    }

    [Route("search")]
    [HttpGet]
    public IActionResult Search(string search)
    {
        throw new NotImplementedException();
    }
}