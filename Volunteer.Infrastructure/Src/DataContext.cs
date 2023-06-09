﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Volunteer.Models;
using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Infrastructure;

public class DataContext : IdentityDbContext<UserIdentity>
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Event>().Navigation(e => e.Participants).AutoInclude();
        builder.Entity<Event>().Navigation(e => e.Admin).AutoInclude();
        
        base.OnModelCreating(builder);
    }
}