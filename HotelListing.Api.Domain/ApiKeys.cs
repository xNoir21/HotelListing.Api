using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.Api.Domain;

public class ApiKeys
{
    public Guid Id { get; set; }
    public string KeyId { get; set; }
    public string KeyHash { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? ExpiresOnUtc { get; set; }
    [NotMapped] public bool IsValid => ExpiresOnUtc.HasValue && ExpiresOnUtc.Value > DateTime.UtcNow;
}