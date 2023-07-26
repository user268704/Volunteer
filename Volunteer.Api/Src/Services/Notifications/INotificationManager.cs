using Volunteer.Models.Requests;

namespace Volunteer.Api.Services.Notifications;

public interface INotificationManager
{
    public void SendNotification(Notification notification);
    public Task<Notification> CreateNotificationAsync(NotificationDto notification);
    public void CheckedNotification(Notification notification);
}