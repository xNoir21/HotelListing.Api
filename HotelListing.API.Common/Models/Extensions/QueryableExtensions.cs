using HotelListing.API.Common.Models.Paging;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Common.Models.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query,
        PaginationParameters paginationParameters)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalCount / (double)paginationParameters.PageSize);

        var metadata = new PaginationMetadata
        {
            CurrentPage = paginationParameters.PageNumber,
            PageSize = paginationParameters.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = paginationParameters.PageNumber < totalPages,
            HasPrevious = paginationParameters.PageNumber > 1
        };
        return new PagedResult<T>
        {
            Metadata = metadata,
            Data = items
        };
    }
}