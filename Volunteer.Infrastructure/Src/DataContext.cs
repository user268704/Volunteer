using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volunteer.Models.Event;
using Volunteer.Models.Requests;
using Volunteer.Models.User;

namespace Volunteer.Infrastructure;

public class DataContext : IdentityDbContext<UserIdentity>
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<ActiveEvents> ActiveUserEvents { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // builder.Entity<Event>().Navigation(e => e.Admin).AutoInclude();
        
        base.OnModelCreating(builder);
    }
}