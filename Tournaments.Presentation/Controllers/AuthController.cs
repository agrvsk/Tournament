using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Tournament.Core.DTOs;

namespace Tournaments.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public AuthController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserForRegistrationDto>>> GetAllUsers() 
    {
        var result = await serviceManager.AuthService.GetAllUsersAsync();
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<ActionResult> RegisterUser(UserForRegistrationDto registrationDto)
    {
        var result = await serviceManager.AuthService.RegisterUserAsync(registrationDto);
        return result.Succeeded ? StatusCode(StatusCodes.Status201Created) : BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Authenticate(UserForAuthDto userForAuthDto)
    {
        if (!await serviceManager.AuthService.ValidateUserAsync(userForAuthDto))
        {
            return Unauthorized();
        }

        // var token = new { Token = await serviceManager.AuthService.CreateTokenAsync() };
        TokenDto token = await serviceManager.AuthService.CreateTokenAsync(expireTime: true);

        return Ok(token);
    }
}
