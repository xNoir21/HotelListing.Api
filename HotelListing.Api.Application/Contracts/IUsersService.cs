using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Application.DTOs.Auth.Register;
using HotelListing.API.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IUsersService
{
    string UserId { get; }
    Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
}