using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel;

public class CreateHotelDto
{
    [Required] [MaxLength(50)] public string Name { get; set; } = null!;

    [Required] [MaxLength(150)] public string Address { get; set; } = null!;

    [Range(1, 5)] public double Rating { get; set; }

    [Required] public int CountryId { get; set; }
}