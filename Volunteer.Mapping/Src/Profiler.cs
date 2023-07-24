using System.Data;
using AutoMapper;
using Volunteer.Models.Event;
using Volunteer.Models.User;

namespace Volunteer.Mapping;

public class Profiler : Profile
{
    public Profiler()
    {
        CreateMap<UserIdentity, UserDto>().ReverseMap();
        CreateMap<UserIdentity, UserRegister>().ReverseMap();
        CreateMap<UserIdentity, UserLogin>().ReverseMap();
        CreateMap<Event, EventCreate>().ReverseMap();
        
        CreateMap<Event, EventDto>()
            .ReverseMap();
    }
}