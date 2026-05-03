using AutoMapper;
using HotelListing.Api.Application.DTOs.Auth.Register;
using HotelListing.Api.Domain;

namespace HotelListing.Api.Application.MappingProfiles;

public class UserMappingProfiles : Profile
{
    public UserMappingProfiles()
    {
        CreateMap<ApplicationUser, RegisteredUserDto>();
        CreateMap<RegisterUserDto, ApplicationUser>()
            .ForMember(
                dest => dest.UserName,
                opt =>
                    opt.MapFrom(src => src.Email)
            );
    }
}