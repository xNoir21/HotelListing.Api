using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.ApiKeys;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiKeysController(IApiKeyService apiKeyService) : BaseApiController
{
    // POST: api/ApiKeys
    [HttpPost]
    public async Task<ActionResult<ReturnApiKeysDto>> PostApiKeys([FromBody] CreateApiKeysDto createApiKeysDto)
    {
        var result = await apiKeyService.CreateApiKeys(createApiKeysDto);
        return ToActionResult(result);
    }
}