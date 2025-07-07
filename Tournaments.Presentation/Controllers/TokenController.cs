using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Tournament.Core.DTOs;

namespace Companies.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAuthService authService;

        public TokenController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken(TokenDto token)
        {
            TokenDto tokenDto = await authService.RefreshTokenAsync(token);
            return Ok(tokenDto);
        }
    }
}
