using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using HotelListing.Api.DTOs.Auth.Register;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace HotelListing.Api.Services;

public class UsersService(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IConfiguration configuration) : IUsersService
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

        await userManager.AddToRoleAsync(user, registerUserDto.Role);

        return Result<RegisteredUserDto>.Success(mapper.Map<RegisteredUserDto>(user));
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto loginUserDto)
    {
        var user = await userManager.FindByEmailAsync(loginUserDto.Email);
        if (user == null) return Result<string>.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
        if (!isPasswordValid) return Result<string>.BadRequest(new Error(ErrorCodes.BadRequest, "Invalid Credentials"));

        //Issue a token
        var token = await GenerateJwtToken(user);

        return Result<string>.Success(token);
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        // Set basic user claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName)
        };
        // Set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(rol => new Claim(ClaimTypes.Role, rol)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT Key Credentails
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration.GetSection("JWTSettings:Key").Value));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            configuration.GetSection("JWTSettings:Issuer").Value,
            configuration.GetSection("JWTSettings:Audience").Value,
            claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(configuration.GetSection("JWTSettings:DurationInMinutes").Value)),
            signingCredentials: credentials
        );

        // Return Token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}