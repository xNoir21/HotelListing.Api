using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth.Register;

namespace HotelListing.Api.MappingProfiles;

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