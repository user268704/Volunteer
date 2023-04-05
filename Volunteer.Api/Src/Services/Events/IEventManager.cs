using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Api.Services.Events;

public interface IEventManager
{
    public Task CreateAsync(Event @event);
    public void AddUser(Event @event, UserIdentity user);
    public void Edit();
    public void Remove(Event @event);
    public Event? Get(Guid eid);
    public List<Event> GetList(int take = -1);
    public List<Event> GetFilter(City city, EventType type);
}