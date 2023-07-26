using Volunteer.Models.User;

namespace Volunteer.Models.Requests;

public class Notification
{
    public Guid Id { get; set; }
    public UserIdentity User { get; set; }
    public string Description { get; set; }
    public string? Link { get; set; }
    public bool IsChecked { get; set; }
    public DateTime DateCreate { get; set; }
}