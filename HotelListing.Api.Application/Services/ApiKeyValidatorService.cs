using System.Security.Cryptography;
using System.Text;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services;

public class ApiKeyValidatorService(HotelListingDbContext context) : IApiKeyValidatorService
{
    public async Task<bool> IsApiKeyValid(string apiKeyId, string apiKeySecret, CancellationToken ct = default)
    {
        var apiKey = await context.ApiKeys.FirstOrDefaultAsync(a => a.KeyId == apiKeyId, ct);
        if (apiKey is null) return false;

        var isSameKeySecret = FixedTimeEquals(
            apiKey.KeyHash,
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(apiKeySecret))
            )
        );

        return isSameKeySecret && apiKey.IsValid;
    }

    private bool FixedTimeEquals(string keyId, string secret)
    {
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromHexString(keyId),
            Convert.FromHexString(secret)
        );
    }
}