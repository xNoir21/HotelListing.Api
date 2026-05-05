using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.API.Common.Models.Paging;
using HotelListing.API.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IBookingService
{
    Task<Result<PagedResult<GetBookingDto>>> AdminGetBookingsForHotelAsync(int hotelId,
        PaginationParameters paginationParameters);

    Task<Result<GetBookingDto>> CreateBookingsAsync(CreateBookingDto createBookingDto);
    Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto updateBookingDto);
    Task<Result> CancelBookingAsync(int hotelId, int bookingId);
    Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
    Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);

    Task<Result<PagedResult<GetBookingDto>>> UserGetBookingsForHotelAsync(int hotelId,
        PaginationParameters paginationParameters);
}