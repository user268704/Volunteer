using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Api.Services.Users;

public interface IUserManager
{
    public List<Event> GetActiveUserEvents(UserIdentity user);
}