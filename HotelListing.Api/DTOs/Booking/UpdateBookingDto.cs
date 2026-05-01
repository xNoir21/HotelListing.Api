using System.ComponentModel.DataAnnotations;
using HotelListing.Api.Results;

namespace HotelListing.Api.DTOs.Booking;

public class UpdateBookingDto : IValidatableObject
{
    [Required] public DateOnly CheckIn { get; set; }
    [Required] public DateOnly CheckOut { get; set; }
    [Required] [Range(1, 10)] public int Guests { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CheckOut <= CheckIn)
            yield return new ValidationResult(
                "Check-out must be after check-in",
                [nameof(CheckOut), nameof(CheckIn)]);

        if (CheckIn < DateOnly.FromDateTime(DateTime.UtcNow))
            yield return new ValidationResult(
                "Check-in must be on or after Today",
                [nameof(CheckIn)]);
    }
}