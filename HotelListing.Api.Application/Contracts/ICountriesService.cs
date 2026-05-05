using HotelListing.Api.Application.DTOs.Country;
using HotelListing.API.Common.Models.Filtering;
using HotelListing.API.Common.Models.Paging;
using HotelListing.API.Common.Results;
using Microsoft.AspNetCore.JsonPatch;

namespace HotelListing.Api.Application.Contracts;

public interface ICountriesService
{
    Task<Result<PagedResult<GetCountriesDto>>> GetCountriesAsync(PaginationParameters paginationParameters,
        CountryFilterParameters filters);

    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto countryDto);
    Task<Result> UpdateCountryAsync(int id, UpdateCountryDto countryDto);
    Task<Result> DeleteCountryAsync(int id);
    Task<bool> CountryExistsAsync(int id);
    Task<bool> CountryExistsAsync(string name);
    Task<Result> PatchCountryAsync(int id, JsonPatchDocument<UpdateCountryDto> patchDocument);
}