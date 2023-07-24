using Volunteer.Infrastructure;
using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Api.Services.Users;

public class UserManager : IUserManager
{
    private readonly DataContext _context;

    public UserManager(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить события в которых участвует пользователь
    /// </summary>
    /// <exception cref="ArgumentNullException">user == null</exception>
    public List<Event> GetActiveUserEvents(UserIdentity user)
    {
        if (user != null)
        {
            return _context.ActiveUserEvents
                .Where(x => x.User.Id == user.Id)
                .Select(x => x.Event)
                .ToList();
        }

        throw new ArgumentNullException("user");
    }
}