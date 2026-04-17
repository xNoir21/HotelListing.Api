using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.Contracts;

public interface IHotelsService
{
    Task<IEnumerable<GetHotelsDto>> GetHotelsAsync();
    Task<GetHotelsDto?> GetHotelAsync(int id);
    Task<GetHotelsDto> CreateHotelAsync(CreateHotelDto hotelDto);
    Task UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    Task DeleteHotelAsync(int id);
    Task<bool> HotelExistsAsync(int id);
    Task<bool> HotelExistsAsync(string name);
}