namespace HotelListing.Api.DTOs.Hotel;

public class UpdateHotelDto : CreateHotelDto
{
    public required int Id { get; set; }
}