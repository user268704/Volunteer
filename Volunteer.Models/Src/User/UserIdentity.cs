using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Volunteer.Models.Event;

namespace Volunteer.Models.User;

public class UserIdentity : IdentityUser
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Lastname { get; set; }
    
    [Required]
    public string Patronymic { get; set; }
    
    [Required]
    public int Age { get; set; }    

    public City? City { get; set; }
}