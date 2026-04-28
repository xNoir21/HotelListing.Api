using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.DTOs.ApiKeys;

public class CreateApiKeysDto
{
    [Required] public string AppName { get; set; } = string.Empty;
}