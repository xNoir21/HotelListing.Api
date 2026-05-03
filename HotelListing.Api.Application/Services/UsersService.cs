using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Application.DTOs.Auth.Register;
using HotelListing.API.Common.Constants;
using HotelListing.API.Common.Models.Configuration;
using HotelListing.API.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HotelListing.Api.Application.Services;

public class UsersService(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IOptions<JwtSettings> jwtOptions,
    IHttpContextAccessor accessor,
    HotelListingDbContext dbContext) : IUsersService
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

        if (registerUserDto.Role == RoleNames.HotelAdmin)
        {
            dbContext.HotelAdmins.Add(
                new HotelAdmin
                {
                    UserId = user.Id,
                    HotelId = registerUserDto.AssociatedhotelId.GetValueOrDefault()
                });
            await dbContext.SaveChangesAsync();
        }

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

    public string UserId => accessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                            accessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                            string.Empty;

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        // Set basic user claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Name, user.FullName)
        };
        // Set user role claims
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(rol => new Claim(ClaimTypes.Role, rol)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT Key Credentails
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            jwtOptions.Value.Issuer,
            jwtOptions.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(jwtOptions.Value.DurationInMinutes)),
            signingCredentials: credentials
        );

        // Return Token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}