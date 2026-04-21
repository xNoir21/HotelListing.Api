namespace HotelListing.Api.DTOs.ApiKeys;

public class ReturnApiKeysDto
{
    public string ApiKey { get; set; }
    public string AppName { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
}