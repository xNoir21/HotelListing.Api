using HotelListing.Api.Constants;
using HotelListing.Api.Results;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected ActionResult ToActionResult(Result result)
    {
        if (result.IsSuccess) return NoContent();

        return result.IsSuccess ? NoContent() : MapErrorToResponse(result.Errors);
    }

    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        return result.IsSuccess ? Ok(result.Value) : MapErrorToResponse(result.Errors);
    }

    protected ActionResult MapErrorToResponse(Error[] errors)
    {
        if (errors.Length == 0) return Problem();
        var error = errors[0];
        return error.Code switch
        {
            ErrorCodes.NotFound => NotFound(error.Description),
            ErrorCodes.Conflict => Conflict(error.Description),
            ErrorCodes.BadRequest => BadRequest(error.Description),
            ErrorCodes.Validation => ValidationProblem(error.Description),
            ErrorCodes.Forbid => Forbid(error.Description),
            _ => Problem(string.Join("; ", errors.Select(x => x.Description)), title: error.Code)
        };
    }
}