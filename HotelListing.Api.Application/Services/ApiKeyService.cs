using System.Security.Cryptography;
using System.Text;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.ApiKeys;
using HotelListing.API.Common.Constants;
using HotelListing.API.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services;

public class ApiKeyService(HotelListingDbContext context) : IApiKeyService
{
    public async Task<Result<ReturnApiKeysDto>> CreateApiKeys(CreateApiKeysDto createApiKeysDto)
    {
        var existingApp = await context.ApiKeys.AnyAsync(a =>
            a.AppName == createApiKeysDto.AppName &&
            a.ExpiresOnUtc > DateTimeOffset.UtcNow
        );

        if (existingApp)
            return Result<ReturnApiKeysDto>.Failure(new Error(
                    ErrorCodes.Conflict,
                    "Valid ApiKey already exists for this app."
                )
            );

        var keyIdBytes = RandomNumberGenerator.GetBytes(6);
        var secretBytes = RandomNumberGenerator.GetBytes(36);

        var keyId = Convert.ToHexString(keyIdBytes).ToLowerInvariant();
        var secret = Convert.ToBase64String(secretBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        var fullKey = $"xnoir_{keyId}.{secret}";
        var keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(secret)));
        var createdOn = DateTimeOffset.UtcNow;

        var apiKeys = new ApiKeys
        {
            AppName = createApiKeysDto.AppName,
            CreatedOnUtc = createdOn,
            ExpiresOnUtc = createdOn.AddDays(60),
            KeyHash = keyHash,
            KeyId = keyId
        };
        context.ApiKeys.Add(apiKeys);
        await context.SaveChangesAsync();

        var newApiKeys = new ReturnApiKeysDto
        {
            AppName = createApiKeysDto.AppName,
            ApiKey = fullKey,
            CreatedOn = createdOn,
            ExpiresOn = createdOn.AddDays(60)
        };

        return Result<ReturnApiKeysDto>.Success(newApiKeys);
    }
}