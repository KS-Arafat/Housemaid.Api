using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Housemaid.api.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class ProtectedController : ControllerBase
{
    [HttpGet("me")]
    public ActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var email = User.FindFirstValue(ClaimTypes.Email);

        var userName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User is not valid.");

        return Ok(new
        {
            isValid = true,
            userId,
            userName,
            email,
            message = "Token is valid."
        });
    }
}
