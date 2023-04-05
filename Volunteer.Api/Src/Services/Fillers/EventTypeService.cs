using Volunteer.Api.Services.Fillers;
using Volunteer.Infrastructure;
using Volunteer.Models.Event;

namespace Volunteer.Api.Services.Events;

public class EventTypeService : IFillService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    
    public EventTypeService(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    
    public void Fill()
    {
        var cities = _configuration.GetSection("EventTypes").GetChildren();

        List<EventType> types = new();

        foreach (IConfigurationSection section in cities)
        {
            types.Add(new EventType
            {
                Id = Guid.Parse(section["Id"]),
                Value = section["value"]
            });
        }


        if (!_context.EventTypes.Any())
        {
            _context.EventTypes.AddRange(types);
            _context.SaveChanges();
        }
    }
}