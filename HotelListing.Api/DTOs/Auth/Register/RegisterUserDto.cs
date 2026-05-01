using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Auth.Register;

public class RegisterUserDto : IValidatableObject
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] [MinLength(8)] public string Password { get; set; } = null!;

    [Required] [MaxLength(100)] public string FirstName { get; set; } = null!;

    [Required] [MaxLength(100)] public string LastName { get; set; } = null!;

    public string Role { get; set; } = "User";
    public int? AssociatedhotelId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Role == "Hotel Admin" && AssociatedhotelId.GetValueOrDefault() < 1)
            yield return new ValidationResult(
                "Please provide a valid Hotel Id",
                new[] { nameof(AssociatedhotelId) });
    }
}