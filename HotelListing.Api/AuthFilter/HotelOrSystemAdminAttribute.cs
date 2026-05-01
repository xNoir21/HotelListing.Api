using System.Security.Claims;
using HotelListing.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace HotelListing.Api.AuthFilter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class HotelOrSystemAdminAttribute() : TypeFilterAttribute(typeof(HotelOrSystemAdminFilter));

public class HotelOrSystemAdminFilter(HotelListingDbContext dbContext) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpUser = context.HttpContext.User;
        if (httpUser?.Identity?.IsAuthenticated == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (httpUser!.IsInRole("Admin")) return;

        var userId = httpUser.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                     httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }

        if (!context.RouteData.Values.TryGetValue("hotelId", out var hotelIdObj))
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (!int.TryParse(hotelIdObj.ToString(), out var hotelId))
        {
            context.Result = new BadRequestResult();
            return;
        }

        if (hotelId == 0)
        {
            context.Result = new ForbidResult();
            return;
        }

        var isUserHotelAdmin = await dbContext.HotelAdmins
            .AnyAsync(b => b.UserId == userId && b.HotelId == hotelId);

        if (!isUserHotelAdmin) context.Result = new ForbidResult();
    }
}