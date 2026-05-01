using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.DTOs.Auth.Register;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts;

public interface IUsersService
{
    Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
    string UserId { get; }
}