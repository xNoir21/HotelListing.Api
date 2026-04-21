using HotelListing.Api.DTOs.ApiKeys;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts;

public interface IApiKeyService
{
    Task<Result<ReturnApiKeysDto>> CreateApiKeys(CreateApiKeysDto createApiKeysDto);
}