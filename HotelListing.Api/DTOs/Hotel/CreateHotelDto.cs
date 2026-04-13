using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.Hotel;

public class CreateHotelDto
{
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [MaxLength(150)]
    public required string Address { get; set; }
    
    [Range(1,5)]
    public double Rating { get; set; }
    
    public required int CountryId { get; set; }
}