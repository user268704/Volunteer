namespace Volunteer.Models.User;

public class ActiveEvents
{
    public Guid Id { get; set; }
    public Event.Event Event { get; set; }
    public UserIdentity User { get; set; }
}