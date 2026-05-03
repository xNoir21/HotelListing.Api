using HotelListing.Api.Application.DTOs.ApiKeys;
using HotelListing.API.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IApiKeyService
{
    Task<Result<ReturnApiKeysDto>> CreateApiKeys(CreateApiKeysDto createApiKeysDto);
}