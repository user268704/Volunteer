using Volunteer.Infrastructure;
using Volunteer.Models.Event;

namespace Volunteer.Api.Services.Fillers;

public class CityService : IFillService
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public CityService(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void Fill()
    {
        var cities = _configuration.GetSection("Cities").GetChildren();

        List<City> citiesList = new();

        foreach (IConfigurationSection section in cities)
        {
            citiesList.Add(new City
            {
                Id = Guid.Parse(section["Id"]),
                Value = section["value"]
            });
        }

        if (!_context.Cities.Any())
        {
            _context.Cities.AddRange(citiesList);
            _context.SaveChanges();
        }
    }
}