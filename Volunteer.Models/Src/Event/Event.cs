using Volunteer.Models.User;

namespace Volunteer.Models.Event;

public class Event
{
    public Guid Id { get; set; }
    public bool IsConfirmed { get; set; }
    public UserIdentity Admin { get; set; }
    public string Name { get; set; }
    public City City { get; set; }
    // Место
    public string Venue { get; set; }
    // Цель
    public string Purpose { get; set; }
    public int NumbParticipants { get; set; }
    public string Inventory { get; set; }
    public string Description { get; set; }
    public EventType Type { get; set; }
    public DateTime Date { get; set; }
}