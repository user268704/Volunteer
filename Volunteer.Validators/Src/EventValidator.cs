using FluentValidation;
using Volunteer.Models.Event;

namespace Volunteer.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(newEvent => newEvent.Name)
            .Length(1, 100)
            .WithMessage("Название мероприятия не может быть таких размеров");

        RuleFor(newEvent => newEvent.Description)
            .Length(0, 3000)
            .WithMessage("Описание не может быть таких размеров");

        RuleFor(newEvent => newEvent.Type)
            .NotNull()
            .WithMessage("Тип мероприятия должен быть указан");

        RuleFor(newEvent => newEvent.City)
            .NotNull()
            .WithMessage("Город проведения мероприятия должен быть указан");

        RuleFor(newEvent => newEvent.Purpose)
            .NotEmpty()
            .WithMessage("Цель мероприятия должна быть указана");

        RuleFor(newEvent => newEvent.Venue)
            .NotEmpty()
            .WithMessage("Место проведения мероприятия должно быть указана");
        
        /*
        RuleFor(newEvent => newEvent.NumbParticipants)
            .LessThan(x => x.Participants.Count)
            .WithMessage("Максимальное количество участников");
    */
    }
}