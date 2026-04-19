using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelService(HotelListingDbContext context, ICountriesService countriesService, IMapper mapper)
    : IHotelsService
{
    public async Task<Result<IEnumerable<GetHotelsDto>>> GetHotelsAsync()
    {
        var hotels = await context.Hotels
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .ToListAsync();
        return Result<IEnumerable<GetHotelsDto>>.Success(hotels);
    }

    public async Task<Result<GetHotelsDto>> GetHotelAsync(int id)
    {
        var hotel = await context.Hotels
            .Where(h => h.Id == id)
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        return hotel is null
            ? Result<GetHotelsDto>.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id}' not found"))
            : Result<GetHotelsDto>.Success(hotel);
    }

    public async Task<Result<GetHotelsDto>> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        try
        {
            var country = await countriesService.CountryExistsAsync(hotelDto.CountryId);

            if (!country)
                return Result<GetHotelsDto>.NotFound(
                    new Error(
                        ErrorCodes.NotFound,
                        $"Country '{hotelDto.CountryId}' not found"
                    )
                );

            var exists = await HotelExistsAsync(hotelDto.Name);

            if (exists)
                return Result<GetHotelsDto>.Failure(
                    new Error(
                        ErrorCodes.Conflict,
                        $"Hotel with the name {hotelDto.Name} already exists."
                    )
                );

            var hotel = mapper.Map<Hotel>((hotelDto));

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            var resultDto = mapper.Map<GetHotelsDto>(hotel);

            return Result<GetHotelsDto>.Success(resultDto);
        }
        catch (Exception e)
        {
            return Result<GetHotelsDto>.Failure(new Error(ErrorCodes.Error, e.Message));
        }
    }

    public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto hotelDto)
    {
        try
        {
            if (id != hotelDto.Id)
                return Result.BadRequest(
                    new Error(
                        ErrorCodes.BadRequest,
                        $"Route ID and body ID don't match"
                    )
                );

            var hotel = await context.Hotels.FindAsync(id);
            if (hotel is null)
                return Result.NotFound(
                    new Error(
                        ErrorCodes.NotFound,
                        $"Hotel '{id}' not found"
                    )
                );

            var country = await countriesService.CountryExistsAsync(hotelDto.CountryId);
            if (!country)
                return Result.NotFound(
                    new Error(
                        ErrorCodes.NotFound,
                        $"Country '{hotelDto.CountryId}' not found"
                    )
                );

            mapper.Map(hotelDto, hotel);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(ErrorCodes.Error, e.Message));
        }
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        try
        {
            var hotel = await context.Hotels.FindAsync(id);
            if (hotel is null)
                return Result.NotFound(
                    new Error(
                        ErrorCodes.NotFound,
                        $"Hotel '{id}' not found"
                    )
                );

            context.Hotels.Remove(hotel);
            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(ErrorCodes.Error, e.Message));
        }
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