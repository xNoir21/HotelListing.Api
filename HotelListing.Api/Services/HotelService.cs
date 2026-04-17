using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelService (HotelListingDbContext context, IMapper mapper) : IHotelsService
{
    public async Task<IEnumerable<GetHotelsDto>> GetHotelsAsync()
    {
        var hotels = await context.Hotels
            .Include(h => h.Country)
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return hotels;
    }

    public async Task<GetHotelsDto?> GetHotelAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .Include(h => h.Country)
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        return hotel;
    }

    public async Task<GetHotelsDto> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var country = await context.Countries.FindAsync(hotelDto.CountryId) ??
                      throw new KeyNotFoundException("Country not found");
        
        var hotel = mapper.Map<Hotel>(hotelDto);

        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();
        
        var resultDto = new GetHotelsDto(
            hotel.Id, 
            hotel.Name, 
            hotel.Address, 
            hotel.Rating, 
            country.Name,
            hotel.CountryId
        );

        return resultDto;
    }

    public async Task UpdateHotelAsync(int id, UpdateHotelDto hotelDto)
    {
        var hotel = await context.Hotels.FindAsync(id) ?? 
                    throw new KeyNotFoundException("Hotel not found");
        
        hotel = mapper.Map(hotelDto, hotel);
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteHotelAsync(int id)
    {
        var hotel = await context.Hotels.FindAsync(id) ??
                    throw new KeyNotFoundException("Hotel not found");

        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync();
    }

    public async Task<bool> HotelExistsAsync(int id)
    {
        return await context.Hotels.AnyAsync(h => h.Id == id);
    }

    public async Task<bool> HotelExistsAsync(string name)
    {
        return await context.Hotels.AnyAsync(h => h.Name == name);
    }
}