﻿using AutoMapper;
using Volunteer.Models.Event;
using Volunteer.Models.Requests;
using Volunteer.Models.User;

namespace Volunteer.Mapping;

public class Profiler : Profile
{
    public Profiler()
    {
        CreateMap<UserIdentity, Guid>().ConvertUsing<AdminToGuidConverter>();
        CreateMap<Guid, UserIdentity>().ConvertUsing<GuidToAdminConverter>();
        CreateMap<City, Guid>().ConvertUsing<CityToGuidConverter>();
        CreateMap<Guid, City>().ConvertUsing<GuidToCityConverter>();
        CreateMap<EventType, Guid>().ConvertUsing<GuidToTypeEventConverter>();
        CreateMap<Guid, EventType>().ConvertUsing<TypeEventToGuidConverter>();
        
        
        CreateMap<UserIdentity, UserDto>().ReverseMap();
        CreateMap<UserIdentity, UserRegister>().ReverseMap();
        CreateMap<UserIdentity, UserLogin>().ReverseMap();
        CreateMap<Event, EventCreate>().ReverseMap();

        CreateMap<ICollection<Event>, ICollection<EventDto>>()
            .ReverseMap();

        CreateMap<Event, EventDto>()
            .ReverseMap();

        CreateMap<Notification, NotificationDto>().ReverseMap();
    }

    #region CustomConverters

    class TypeEventToGuidConverter : ITypeConverter<Guid, EventType>
    {
        public EventType Convert(Guid source, EventType destination, ResolutionContext context)
        {
            if (context.Items.TryGetValue("eventType", out var city))
                return (EventType)city;

            return destination;
        }
    }

    class AdminToGuidConverter : ITypeConverter<UserIdentity, Guid>
    {
        public Guid Convert(UserIdentity source, Guid destination, ResolutionContext context)
        {
            return Guid.Parse(source.Id);
        }
    }

    class GuidToAdminConverter : ITypeConverter<Guid, UserIdentity>
    {
        public UserIdentity Convert(Guid source, UserIdentity destination, ResolutionContext context)
        {
            UserIdentity admin = (UserIdentity)context.Items["admin"];

            return admin;
        }
    }
    
    
    class GuidToTypeEventConverter : ITypeConverter<EventType, Guid>
    {
        public Guid Convert(EventType source, Guid destination, ResolutionContext context)
        {
            return source.Id;
        }
    }

    class GuidToCityConverter : ITypeConverter<Guid, City>
    {
        public City Convert(Guid source, City destination, ResolutionContext context)
        {
            if (context.Items.TryGetValue("city", out var city))
                return (City)city;

            return destination;
        }
    }

    class CityToGuidConverter : ITypeConverter<City, Guid>
    {
        public Guid Convert(City destination, Guid source, ResolutionContext context)
        {
            return ((City)context.Items["city"]).Id;
        }
    }

    #endregion
}