using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.API.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IHotelsService
{
    Task<Result<IEnumerable<GetHotelsDto>>> GetHotelsAsync();
    Task<Result<GetHotelsDto>> GetHotelAsync(int id);
    Task<Result<GetHotelsDto>> CreateHotelAsync(CreateHotelDto hotelDto);
    Task<Result> UpdateHotelAsync(int id, UpdateHotelDto hotelDto);
    Task<Result> DeleteHotelAsync(int id);
    Task<bool> HotelExistsAsync(int id);
    Task<bool> HotelExistsAsync(string name);
}