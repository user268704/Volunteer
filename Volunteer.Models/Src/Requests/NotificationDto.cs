namespace Volunteer.Models.Requests;

public class NotificationDto
{
    public Guid User { get; set; }
    public string Description { get; set; }
    public string? Link { get; set; }
}