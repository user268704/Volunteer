using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Volunteer.Api.Controllers;
using Volunteer.Api.Services.Events;
using Volunteer.Infrastructure;
using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Tests.Tests.Controllers;

public class EventsControllerTest
{

    public EventsController Controller { get; set; }
    
    public EventsControllerTest()
    {
        /*
        var eventManagerFake = new Mock<IEventManager>();
        var userManagerFake = new Mock<UserManager<UserIdentity>>();
        var dataContextFake = new Mock<DataContext>();
        var mapperFake = new Mock<IMapper>();
        
        Controller = new(eventManagerFake.Object, userManagerFake.Object, dataContextFake.Object, mapperFake.Object);
    */
    }
    
    [Fact]
    public async Task CreateTest()
    {
        // Arrange
        var mock = new Mock<IEventManager>();
        mock.Setup(x => x.CreateAsync(CreateEvent()));

        // Act
        var createResult = await Controller.Create(new EventCreate());

        await createResult.ExecuteResultAsync(new ControllerContext());
        
        // Assert
        
        Assert.NotNull(createResult);
    }

    private Event CreateEvent()
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            Name = "dasf"
        };
    }
    
}