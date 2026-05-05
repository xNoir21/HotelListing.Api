namespace HotelListing.API.Common.Models.Filtering;

public abstract class BaseFilterParameters
{
    public string? Search { get; set; }
    public string? OrderBy { get; set; }
    public bool SortDescending { get; set; } = false;
}