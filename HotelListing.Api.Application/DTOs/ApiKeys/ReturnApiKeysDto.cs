namespace HotelListing.Api.Application.DTOs.ApiKeys;

public class ReturnApiKeysDto
{
    public string ApiKey { get; set; } = null!;
    public string AppName { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
}