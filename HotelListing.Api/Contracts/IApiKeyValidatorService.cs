namespace HotelListing.Api.Contracts;

public interface IApiKeyValidatorService
{
    Task<bool> IsApiKeyValid(string apiKeyId, string apiKeySecret, CancellationToken ct = default);
}