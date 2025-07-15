using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Requests;

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
    [Authorize]
    //[Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    //[Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<IEnumerable<UserForRegistrationDto>>> GetAllUsers([FromQuery]UserRequestParams uParams) 
    {
        if (User?.Identity?.IsAuthenticated ?? false)
        {
//            return Ok("Is Auth");
            var result = await serviceManager.AuthService.GetAllUsersAsync(uParams);
            return Ok(result.GetOkResult<IEnumerable<UserForRegistrationDto>>());
        }
        else
        {
            return BadRequest("Is not auth");
        }

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
