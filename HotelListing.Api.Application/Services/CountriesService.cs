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

namespace HotelListing.Api.Application.Services;

public class CountriesService(HotelListingDbContext context, IMapper mapper) : ICountriesService
{
    public async Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(PaginationParameters paginationParameters,
        CountryFilterParameters filters)
    {
        var query = context.Countries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%") ||
                                     EF.Functions.Like(c.ShortName, $"%{term}%"));
        }

        var countries = await query
            .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        return Result<PagedResult<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.CountryId == id)
            .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
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

        var duplicateExists = await context.Countries.AnyAsync(c =>
            c.Name.ToLower().Trim() == countryDto.Name.ToLower().Trim() &&
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