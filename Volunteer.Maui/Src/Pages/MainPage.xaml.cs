using System.Collections.ObjectModel;
using Volunteer.Models.Event;
using Toast = CommunityToolkit.Maui.Alerts.Toast;

namespace Volunteer.Maui.Pages;

public partial class MainPage : ContentPage
{

    public ObservableCollection<EventDto> Events { get; set; }
    
    public MainPage()
    {
        InitializeComponent();
        FillList();
        
        BindingContext = this;
    }
    
    private void FillList()
    {
        Events = new()
        {
            new EventDto()
            {
                Type = Guid.NewGuid(),
                Admin = Guid.NewGuid(),
                City = Guid.NewGuid(),
                Description = "Базовое описание с каким то текстом",
                Inventory = "Какой то инвентарь",
                Name = "Помогаем бездомным",
                Participants = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                Purpose = "Цель мероприятия",
                Venue = "Место проведения мероприятия",
                IsConfirmed = true,
                NumbParticipants = 6,
                Date = DateTime.Now
            },
            new EventDto()
            {
                Type = Guid.NewGuid(),
                Admin = Guid.NewGuid(),
                City = Guid.NewGuid(),
                Description = "Базовое описание с каким то текстом",
                Inventory = "Какой то инвентарь",
                Name = "Помогаем бездомным",
                Participants = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                Purpose = "Цель мероприятия",
                Venue = "Место проведения мероприятия",
                IsConfirmed = true,
                NumbParticipants = 6,
                Date = DateTime.Now
            },
            new EventDto()
            {
                Type = Guid.NewGuid(),
                Admin = Guid.NewGuid(),
                City = Guid.NewGuid(),
                Description = "Базовое описание с каким то текстом",
                Inventory = "Какой то инвентарь",
                Name = "Помогаем бездомным",
                Participants = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                Purpose = "Цель мероприятия",
                Venue = "Место проведения мероприятия",
                IsConfirmed = true,
                NumbParticipants = 6,
                Date = DateTime.Now
            },
            new EventDto()
            {
                Type = Guid.NewGuid(),
                Admin = Guid.NewGuid(),
                City = Guid.NewGuid(),
                Description = "Базовое описание с каким то текстом",
                Inventory = "Какой то инвентарь",
                Name = "Помогаем бездомным",
                Participants = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                Purpose = "Цель мероприятия",
                Venue = "Место проведения мероприятия",
                IsConfirmed = true,
                NumbParticipants = 6,
                Date = DateTime.Now
            },
            new EventDto()
            {
                Type = Guid.NewGuid(),
                Admin = Guid.NewGuid(),
                City = Guid.NewGuid(),
                Description = "Базовое описание с каким то текстом",
                Inventory = "Какой то инвентарь",
                Name = "Помогаем бездомным",
                Participants = new List<Guid>()
                {
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    Guid.NewGuid()
                },
                Purpose = "Цель мероприятия",
                Venue = "Место проведения мероприятия",
                IsConfirmed = true,
                NumbParticipants = 6,
                Date = DateTime.Now
            },
        };
    }

    private void ListView_OnRefreshing(object sender, EventArgs e)
    {
        var toast = Toast.Make("List view refresh");

        toast.Show();
    }
}