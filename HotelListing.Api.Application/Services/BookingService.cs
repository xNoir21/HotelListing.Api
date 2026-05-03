using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Booking;
using HotelListing.API.Common.Constants;
using HotelListing.API.Common.Results;
using HotelListing.Api.Domain;
using HotelListing.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services;

public class BookingService(HotelListingDbContext context, IUsersService usersService, IMapper mapper)
    : IBookingService
{
    public async Task<Result<ICollection<GetBookingDto>>> AdminGetBookingsForHotelAsync(int hotelId)
    {
        var hotel = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotel)
            return Result<ICollection<GetBookingDto>>
                .Failure(new Error("Failure", $"Hotel with id {hotelId} not found."));

        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<ICollection<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<ICollection<GetBookingDto>>> UserGetBookingsForHotelAsync(int hotelId)
    {
        var userId = usersService.UserId;

        var hotel = await context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotel)
            return Result<ICollection<GetBookingDto>>
                .Failure(new Error("Failure", $"Hotel with id {hotelId} not found."));

        var bookings = await context.Bookings
            .Where(b => b.HotelId == hotelId && b.UserId == userId)
            .OrderBy(b => b.CheckIn)
            .ProjectTo<GetBookingDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<ICollection<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> CreateBookingsAsync(CreateBookingDto createBookingDto)
    {
        var userId = usersService.UserId;

        var overlaps = await isOverlap(createBookingDto.HotelId, createBookingDto.CheckIn, createBookingDto.CheckOut,
            userId);
        if (overlaps)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict,
                "The selected dates overlap with an existing booking"));

        var hotel = await context.Hotels
            .FirstOrDefaultAsync(h => h.Id == createBookingDto.HotelId);
        if (hotel is null)
            return Result<GetBookingDto>.BadRequest(new Error(ErrorCodes.BadRequest,
                $"Hotel '{createBookingDto.HotelId}' was not found"));

        var nights = createBookingDto.CheckOut.DayNumber - createBookingDto.CheckIn.DayNumber;

        var booking = mapper.Map<Booking>(createBookingDto);
        booking.TotalPrice = hotel.PerNightRate * nights;
        booking.Status = BookingStatusEnum.Pending;
        booking.UserId = userId;

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var created = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(created);
    }

    public async Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId,
        UpdateBookingDto updateBookingDto)
    {
        var userId = usersService.UserId;

        var overlaps = await isOverlap(hotelId, updateBookingDto.CheckIn, updateBookingDto.CheckOut, userId, bookingId);
        if (overlaps)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure,
                "The selected dates overlap with an existing booking"));

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId && b.HotelId == hotelId);

        if (booking is null)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure,
                $"Hotel '{hotelId}' was not found"));
        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Failure,
                "Canceled bookings cannot be modified"));

        var nights = updateBookingDto.CheckOut.DayNumber - updateBookingDto.CheckIn.DayNumber;
        booking.TotalPrice = booking.Hotel!.PerNightRate * nights;
        booking.UpdatedAtUtc = DateTime.UtcNow;

        mapper.Map(updateBookingDto, booking);

        await context.SaveChangesAsync();

        var updated = mapper.Map<GetBookingDto>(booking);
        return Result<GetBookingDto>.Success(updated);
    }

    public async Task<Result> CancelBookingAsync(int hotelId, int bookingId)
    {
        var userId = usersService.UserId;

        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId && b.HotelId == hotelId);

        if (booking is null)
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"Booking '{bookingId}' was not found"));
        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result.Failure(new Error(ErrorCodes.Failure,
                "The booking has already been canceled"));

        booking.Status = BookingStatusEnum.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId)
    {
        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

        if (booking is null)
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"Booking '{bookingId}' was not found"));
        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result.Failure(new Error(ErrorCodes.Failure,
                "The booking has already been canceled"));

        booking.Status = BookingStatusEnum.Confirmed;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId)
    {
        var booking = await context.Bookings
            .Include(b => b.Hotel)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.HotelId == hotelId);

        if (booking is null)
            return Result.Failure(new Error(ErrorCodes.Failure,
                $"Booking '{bookingId}' was not found"));
        if (booking.Status == BookingStatusEnum.Cancelled)
            return Result.Failure(new Error(ErrorCodes.Failure,
                "The booking has already been canceled"));

        booking.Status = BookingStatusEnum.Cancelled;
        booking.UpdatedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<bool> isOverlap(int hotelId, DateOnly checkIn, DateOnly checkOut, string userId,
        int? bookingId = null)
    {
        var query = context.Bookings.Where(b => b.HotelId == hotelId &&
                                                b.Status != BookingStatusEnum.Cancelled &&
                                                checkIn < b.CheckOut &&
                                                checkOut > b.CheckIn &&
                                                b.UserId == userId).AsQueryable();
        if (bookingId.HasValue) query.Where(b => b.Id != bookingId.Value);
        return await query.AnyAsync();
    }
}