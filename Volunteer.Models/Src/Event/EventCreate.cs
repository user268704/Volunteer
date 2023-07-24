namespace Volunteer.Models.Event;

public class EventCreate
{
    public string Name { get; set; }
    public Guid City { get; set; }
    public string Venue { get; set; }
    public string Purpose { get; set; }
    public string Inventory { get; set; }
    public string Description { get; set; }
    public Guid Type { get; set; }
    public DateTime Date { get; set; }

}