using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await context.Countries
            .Select(c => new GetCountriesDto(c.CountryId, c.Name, c.ShortName))
            .ToListAsync();
        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await context.Countries
            .Where(c => c.CountryId == id)
            .Select(c => new GetCountryDto(
                c.CountryId,
                c.Name,
                c.ShortName,
                c.Hotels.Select(h => new GetHotelSlimDto(
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Rating)
                ).ToList()
            ))
            .FirstOrDefaultAsync();
        return country is null ? Result<GetCountryDto>.NotFound() : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto)
    {
        try
        {
            var exists = await CountryExistsAsync(countryDto.Name);
            if (exists)
                return Result<GetCountryDto>.Failure(
                    new Error(
                        "Conflict",
                        $"Country with the name {countryDto.Name} already exists."
                    )
                );
            var country = new Country
            {
                Name = countryDto.Name,
                ShortName = countryDto.ShortName
            };

            context.Countries.Add(country);
            await context.SaveChangesAsync();

            var resultDto = new GetCountryDto(
                country.CountryId,
                country.Name,
                country.ShortName,
                []
            );
            return Result<GetCountryDto>.Success(resultDto);
        }
        catch (Exception e)
        {
            return Result<GetCountryDto>.Failure(new Error("Error", e.Message));
        }
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        try
        {
            if (id != countryDto.CountryId)
                return Result.BadRequest(new Error("Validation", "Route ID and body ID don't match"));

            var country = await context.Countries.FindAsync(id);
            if (country is null) return Result.NotFound(new Error("NotFound", $"Country '{id}' not found"));

            var exists = await CountryExistsAsync(country.Name);
            if (exists)
                return Result.Failure(
                    new Error(
                        "Conflict",
                        $"Country with the name {countryDto.Name} already exists."
                    )
                );

            country.Name = countryDto.Name;
            country.ShortName = countryDto.ShortName;

            await context.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Error", e.Message));
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await context.Countries.FindAsync(id);
            if (country is null) return Result.NotFound(new Error("NotFound", $"Country '{id}' not found"));

            context.Countries.Remove(country);
            await context.SaveChangesAsync();
            
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error("Error", e.Message));
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
}