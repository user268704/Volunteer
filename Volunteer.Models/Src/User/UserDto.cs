using Volunteer.Models.Event;

namespace Volunteer.Models.User;

public class UserDto
{
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Patronymic { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
    public City? City { get; set; }
}