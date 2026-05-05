using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.API.Common.Constants;
using HotelListing.API.Common.Models.Extensions;
using HotelListing.API.Common.Models.Filtering;
using HotelListing.API.Common.Models.Paging;
using HotelListing.API.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services;

public class HotelService(HotelListingDbContext context, ICountriesService countriesService, IMapper mapper)
    : IHotelsService
{
    public async Task<Result<PagedResult<GetHotelsDto>>> GetHotelsAsync(PaginationParameters paginationParameters,
        HotelFilteringParameters filters)
    {
        var query = context.Hotels.AsQueryable();

        if (filters.CountryId.HasValue) query = query.Where(h => h.CountryId == filters.CountryId);
        if (filters.MinPrice.HasValue) query = query.Where(h => h.PerNightRate >= filters.MinPrice.Value);
        if (filters.MaxPrice.HasValue) query = query.Where(h => h.PerNightRate <= filters.MaxPrice.Value);
        if (filters.MinRating.HasValue) query = query.Where(h => h.Rating <= filters.MinRating.Value);
        if (filters.MaxRating.HasValue) query = query.Where(h => h.Rating <= filters.MaxRating.Value);
        if (!string.IsNullOrWhiteSpace(filters.Location))
            query = query.Where(h => h.Address.Contains(filters.Location));
        // generic search param
        if (!string.IsNullOrWhiteSpace(filters.Search)) query = query.Where(h => h.Name.Contains(filters.Search));

        query = filters.OrderBy?.ToLower() switch
        {
            "name" => filters.SortDescending ? query.OrderByDescending(h => h.Name) : query.OrderBy(h => h.Name),
            "address" => filters.SortDescending
                ? query.OrderByDescending(h => h.Address)
                : query.OrderBy(h => h.Address),
            "rating" => filters.SortDescending ? query.OrderByDescending(h => h.Rating) : query.OrderBy(h => h.Rating),
            "PerNightRating" => filters.SortDescending
                ? query.OrderByDescending(h => h.PerNightRate)
                : query.OrderBy(h => h.PerNightRate),
            _ => query.OrderBy(h => h.Name)
        };

        var hotels = await query
            .ProjectTo<GetHotelsDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);
        return Result<PagedResult<GetHotelsDto>>.Success(hotels);
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

            var hotel = mapper.Map<Hotel>(hotelDto);

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
                        "Route ID and body ID don't match"
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