using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Auth.Register;

public class RegisterUserDto
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] [MinLength(8)] public string Password { get; set; } = null!;

    [Required] [MaxLength(100)] public string FirstName { get; set; } = null!;

    [Required] [MaxLength(100)] public string LastName { get; set; } = null!;

    public string Role { get; set; } = "User";
}