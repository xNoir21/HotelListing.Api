using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.API.Common.Constants;
using HotelListing.API.Common.Models.Extensions;
using HotelListing.API.Common.Models.Filtering;
using HotelListing.API.Common.Models.Paging;
using HotelListing.API.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HotelListing.Api.Application.Services;

public class CountriesService(HotelListingDbContext context, IMapper mapper, IMemoryCache cache) : ICountriesService
{
    public async Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(PaginationParameters paginationParameters,
        CountryFilterParameters filters)
    {
        var searchTerm = filters.Search?.Trim().ToLowerInvariant() ?? string.Empty;
        var cacheKey = $"Countries-List-{searchTerm}-{paginationParameters.PageNumber}-{paginationParameters.PageSize}";

        if (!cache.TryGetValue(cacheKey, out PagedResult<GetCountriesDto>? countries))
        {
            var query = context.Countries.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var term = filters.Search.Trim();
                query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%") ||
                                         EF.Functions.Like(c.ShortName, $"%{term}%"));
            }

            countries = await query
                .AsNoTracking()
                .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
                .ToPagedResultAsync(paginationParameters);
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            cache.Set(cacheKey, countries, cacheEntryOptions);
        }
        
        countries ??= new PagedResult<GetCountriesDto>();

        return Result<PagedResult<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        // In Memory Cache
        // Check the cache
        var cacheKey = $"Country-{id}";
        if (!cache.TryGetValue(cacheKey, out GetCountryDto? country))
        {
            // if not in cache, get from db and add to cache
            country = await context.Countries
                .AsNoTracking()
                .Where(c => c.CountryId == id)
                .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (country != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                cache.Set(cacheKey, country, cacheEntryOptions);
            }
        }

        return country is null
            ? Result<GetCountryDto>.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' not found"))
            : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
    {
        try
        {
            var exists = await CountryExistsAsync(countryDto.Name);
            if (exists)
                return Result<GetCountryDto>.Failure(
                    new Error(
                        ErrorCodes.Conflict,
                        $"Country with the name {countryDto.Name} already exists."
                    )
                );

            var country = mapper.Map<Country>(countryDto);

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var resultDto = mapper.Map<GetCountryDto>(country);

            return Result<GetCountryDto>.Success(resultDto);
        }
        catch (Exception e)
        {
            return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Error, e.Message));
        }
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        try
        {
            if (id != countryDto.CountryId)
                return Result.BadRequest(new Error(ErrorCodes.Validation, "Route ID and body ID don't match"));

            var country = await context.Countries.FindAsync(id);
            if (country is null) return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' not found"));

            mapper.Map(countryDto, country);

            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(ErrorCodes.Error, e.Message));
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);
            if (country is null) return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' not found"));

            context.Countries.Remove(country);
            await context.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(ErrorCodes.Error, e.Message));
        }
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.CountryId == id);
    }

    public async Task<bool> CountryExistsAsync(string name)
    {
        return await context.Countries
            .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public async Task<Result> PatchCountryAsync(int id, JsonPatchDocument<UpdateCountryDto> patchDocument)
    {
        var country = await context.Countries.FindAsync(id);
        if (country is null) return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' not found"));

        var countryDto = mapper.Map<UpdateCountryDto>(country);
        patchDocument.ApplyTo(countryDto);

        if (countryDto.CountryId != id)
            return Result.BadRequest(new Error(ErrorCodes.Validation, "Cannot modify the Id of the country"));

        var normalizedName = countryDto.Name.ToLower().Trim();
        var duplicateExists = await context.Countries.AnyAsync(c =>
            c.Name.ToLower().Trim() == normalizedName &&
            c.CountryId != id
        );

        if (duplicateExists)
            return Result.Failure(new Error(ErrorCodes.Conflict,
                $"Country with name {countryDto.Name} already exists"));

        mapper.Map(countryDto, country);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}