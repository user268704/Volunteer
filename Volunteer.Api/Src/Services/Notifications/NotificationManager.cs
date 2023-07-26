using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Volunteer.Api.Signal;
using Volunteer.Infrastructure;
using Volunteer.Models.Requests;
using Volunteer.Models.User;

namespace Volunteer.Api.Services.Notifications;

public class NotificationManager : INotificationManager
{
    private readonly DataContext _context;
    private readonly UserManager<UserIdentity> _userManager;
    private readonly IHubContext<SignalHub> _signal;

    public NotificationManager(DataContext context, IHubContext<SignalHub> signal,
        UserManager<UserIdentity> userManager)
    {
        _context = context;
        _userManager = userManager;
        _signal = signal;
    }

    public void SendNotification(Notification notification)
    {
        _context.Notifications.Add(notification);

        _signal.Clients.Client(notification.User.Id).SendAsync("checkNotification", notification);
    }

    public async Task<Notification> CreateNotificationAsync(NotificationDto notification)
    {
        Notification fullNotification = new Notification
        {
            IsChecked = false,
            Description = notification.Description,
            User = await _userManager.FindByIdAsync(notification.User.ToString()),
            Link = notification.Link,
            DateCreate = DateTime.Now
        };

        return fullNotification;
    }

    public void CheckedNotification(Notification notification)
    {
        _context.Notifications.Update(notification);

        notification.IsChecked = true;

        _context.SaveChanges();
    }
}