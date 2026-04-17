using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(ICountriesService countriesService) : ControllerBase
{
    // GET: api/Countries
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var result = await countriesService.GetCountriesAsync();
        return ToActionResult(result);
    }

    // GET: api/Countries/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var result = await countriesService.GetCountryAsync(id);
        return ToActionResult(result);
    }

    // PUT: api/Countries/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, [FromBody] UpdateCountryDto countryDto)
    {
        var result = await countriesService.UpdateCountryAsync(id, countryDto);
        return ToActionResult(result);
    }

    // POST: api/Countries
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CreateCountryDto>> PostCountry([FromBody] CreateCountryDto countryDto)
    {
        var result = await countriesService.CreateCountryAsync(countryDto);
        if (!result.IsSuccess) return MapErrorToResponse(result.Errors);
        return CreatedAtAction(nameof(GetCountry), new { id = result.Value!.CountryId }, result.Value);
    }

    // DELETE: api/Countries/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var result = await countriesService.DeleteCountryAsync(id);
        return ToActionResult(result);
    }

    private ActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess) return NoContent();

        return result.IsSuccess ? NoContent() : MapErrorToResponse(result.Errors);
    }

    private ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? Ok(result.Value) : MapErrorToResponse(result.Errors);
    }

    private ActionResult MapErrorToResponse(Error[] errors)
    {
        if (errors.Length == 0) return Problem();
        var error = errors[0];
        return error.Code switch
        {
            "NotFound" => NotFound(error.Description),
            "Conflict" => Conflict(error.Description),
            "BadRequest" => BadRequest(error.Description),
            "Validation" => ValidationProblem(error.Description),
            _ => Problem(string.Join("; ", errors.Select(x => x.Description)), title: error.Code)
        };
    }
}