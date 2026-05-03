namespace HotelListing.API.Common.Models.Configuration;

public class JwtSettings
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int DurationInMinutes { get; set; }
}