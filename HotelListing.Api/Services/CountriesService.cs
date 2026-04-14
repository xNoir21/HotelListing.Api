using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListingApi.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    public async Task<IEnumerable<GetCountriesDto>> GetCountriesAsync()
    {
        var countries = await context.Countries
            .Select(c => new GetCountriesDto(c.CountryId, c.Name, c.ShortName))
            .ToListAsync();
        return countries;
    }

    public async Task<GetCountryDto?> GetCountryAsync(int id)
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
        return country;
    }
    
    public async Task<GetCountryDto> CreateCountryAsync(CreateCountryDto  countryDto)
    {
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
        return resultDto;
    }

    public async Task UpdateCountryAsync(int id, UpdateCountryDto countryDto)
    {
        var country = await context.Countries.FindAsync(id) ?? 
                      throw new KeyNotFoundException("Country not found");
        
        country.Name = countryDto.Name;
        country.ShortName = countryDto.ShortName;
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteCountryAsync(int id)
    {
        var country = await context.Countries.FindAsync(id) ?? 
                      throw new KeyNotFoundException("Country not found");

        context.Countries.Remove(country);
        await context.SaveChangesAsync();
    }
    
    public async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.CountryId == id);
    }
    
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await context.Countries.AnyAsync(e => e.Name == name);
    }
}