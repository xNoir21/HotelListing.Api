using AutoMapper;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.DTOs.Auth.Register;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Services;

public class UsersService(UserManager<ApplicationUser> userManager, IMapper mapper) : IUsersService
{
    public async Task<Result<RegisteredUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
    {
        var user = mapper.Map<ApplicationUser>(registerUserDto);

        var result = await userManager.CreateAsync(user, registerUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(x => new Error(
                    ErrorCodes.BadRequest,
                    x.Description
                )
            ).ToArray();
            return Result<RegisteredUserDto>.BadRequest(errors);
        }
        
        return Result<RegisteredUserDto>.Success(mapper.Map<RegisteredUserDto>(user));
    }

    public async Task<Result> LoginAsync(LoginUserDto loginUserDto)
    {
        var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null)
        {
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }
        
        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid)
        {
            return Result.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));
        }

        return Result.Success();
    }
}