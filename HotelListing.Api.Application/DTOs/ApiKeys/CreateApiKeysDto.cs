using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Application.DTOs.ApiKeys;

public class CreateApiKeysDto
{
    [Required] public string AppName { get; set; } = string.Empty;
}