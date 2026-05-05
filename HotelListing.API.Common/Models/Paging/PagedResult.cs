namespace HotelListing.API.Common.Models.Paging;

public class PagedResult<T>
{
    public PaginationMetadata Metadata { get; set; } = new();
    public ICollection<T> Data { get; set; } = new List<T>();
}