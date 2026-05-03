namespace HotelListing.Api.Application.Contracts;

public interface IApiKeyValidatorService
{
    Task<bool> IsApiKeyValid(string apiKeyId, string apiKeySecret, CancellationToken ct = default);
}