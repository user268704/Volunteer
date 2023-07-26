using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Volunteer.Api.Services.Users;

namespace Volunteer.Api.Signal;

public class SignalHub : Hub
{
    private readonly IUserManager _customUserManager;
    private readonly IMapper _mapper;

    public SignalHub(IUserManager customUserManager, IMapper mapper)
    {
        _customUserManager = customUserManager;
        _mapper = mapper;
    }

    public void SendNotification(Guid notificationId, Guid userId)
    {
    }
}