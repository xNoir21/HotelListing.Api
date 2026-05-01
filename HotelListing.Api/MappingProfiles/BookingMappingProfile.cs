using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Booking;

namespace HotelListing.Api.MappingProfiles;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<CreateBookingDto, Booking>()
            .ForMember(b => b.Id, opt => opt.Ignore())
            .ForMember(b => b.UserId, opt => opt.Ignore())
            .ForMember(b => b.TotalPrice, opt => opt.Ignore())
            .ForMember(b => b.Status, opt => opt.Ignore())
            .ForMember(b => b.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(b => b.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(b => b.Hotel, opt => opt.Ignore());
        CreateMap<Booking, GetBookingDto>()
            .ForMember(b => b.HotelName, opt => opt.MapFrom(b => b.Hotel!.Name))
            .ForMember(b => b.Status, opt => opt.MapFrom(b => b.Status.ToString()));
        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(b => b.Id, opt => opt.Ignore())
            .ForMember(b => b.UserId, opt => opt.Ignore())
            .ForMember(b => b.TotalPrice, opt => opt.Ignore())
            .ForMember(b => b.Status, opt => opt.Ignore())
            .ForMember(b => b.CreatedAtUtc, opt => opt.Ignore())
            .ForMember(b => b.UpdatedAtUtc, opt => opt.Ignore())
            .ForMember(b => b.Hotel, opt => opt.Ignore());
    }
}