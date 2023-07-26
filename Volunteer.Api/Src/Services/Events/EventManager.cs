using Volunteer.Infrastructure;
using Volunteer.Models.Event;
using Volunteer.Models.User;
using Volunteer.Validators;

namespace Volunteer.Api.Services.Events;

public class EventManager : IEventManager
{
    private readonly DataContext _context;
    private EventValidator _eventValidator;

    public EventManager(DataContext context)
    {
        _context = context;
        _eventValidator = new();
    }

    public async Task CreateAsync(Event @event)
    {
        @event.Id = Guid.NewGuid();
        _context.Events.Add(@event);

        await _context.SaveChangesAsync();
    }

    public void AddUser(Event @event, UserIdentity user)
    {
        _context.ActiveUserEvents.Add(new ActiveEvents
        {
            Event = @event,
            User = user
        });

        _context.SaveChanges();
    }

    public void Edit()
    {
        throw new NotImplementedException();
    }

    public void Remove(Event @event)
    {
        _context.Events.Remove(@event);
        _context.SaveChanges();
    }

    public Event? Get(Guid eid)
    {
        return _context.Events.Find(eid);
    }

    public List<Event> GetFilter(City city, EventType type)
    {
        var events = _context.Events
            .Where(x => x.City.Id == city.Id && x.Type.Id == type.Id)
            .ToList();

        return events;
    }

    public List<Event> GetList(int take = -1)
    {
        if (take == -1)
            return _context.Events.ToList();

        return _context.Events.Take(take).ToList();
    }
}