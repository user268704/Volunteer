using Volunteer.Models.User;

namespace Volunteer.Models.Event;

public class EventDto
{
    public bool IsConfirmed { get; set; }
    public Guid Admin { get; set; }
    public List<Guid> Participants { get; set; }
    
    public string Name { get; set; }
    public Guid City { get; set; }
    public string Venue { get; set; }
    public string Purpose { get; set; }
    public int NumbParticipants { get; set; }
    public string Inventory { get; set; }
    public string Description { get; set; }
    public Guid Type { get; set; }
    public DateTime Date { get; set; }
}