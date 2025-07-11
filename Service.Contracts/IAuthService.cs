using Microsoft.AspNetCore.Identity;
using Tournament.Core.DTOs;
using Tournament.Core.Requests;
using Tournament.Core.Responses;

namespace Service.Contracts
{
    public interface IAuthService
    {
        Task<TokenDto> CreateTokenAsync(bool expireTime);
        Task<ApiBaseResponse> GetAllUsersAsync(UserRequestParams uParams);
        Task<TokenDto> RefreshTokenAsync(TokenDto token);
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto registrationDto);
        Task<bool> ValidateUserAsync(UserForAuthDto userForAuthDto);
    }
}