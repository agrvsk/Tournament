using Microsoft.AspNetCore.Identity;
using Tournament.Core.DTOs;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<TokenDto> CreateTokenAsync(bool expireTime);
        Task<ResultObjectDto<IEnumerable<UserForRegistrationDto>>> GetAllUsersAsync();
        Task<TokenDto> RefreshTokenAsync(TokenDto token);
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto registrationDto);
        Task<bool> ValidateUserAsync(UserForAuthDto userForAuthDto);
    }
}