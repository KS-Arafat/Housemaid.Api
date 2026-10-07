using Housemaid.api.DTO;
using Housemaid.api.Models;
using Housemaid.api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Housemaid.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserManager<User> userManager, ITokenService tokenService) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);


        var existingUser = await _userManager.FindByEmailAsync(request.Email)
            ?? await _userManager.FindByNameAsync(request.UserName);

        if (existingUser is not null)
            return Conflict("A user with this email or username already exists.");


        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors.Select(e => e.Description));

        var accessToken = _tokenService.CreateAccessToken(user);

        return Ok(new
        {
            message = "User registered successfully.",
            userId = user.Id,
            userName = user.UserName,
            email = user.Email,
            token = accessToken
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginReqDto request)
    {
        if (!ModelState.IsValid || request.Email is null)
            return ValidationProblem(ModelState);


        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Unauthorized("Invalid Email");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Unauthorized("Invalid Password.");

        var accessToken = _tokenService.CreateAccessToken(user);

        return Ok(new
        {
            message = "Login successful.",
            userId = user.Id,
            userName = user.UserName,
            email = user.Email,
            token = accessToken
        });
    }
}
