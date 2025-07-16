using Microsoft.AspNetCore.Identity;
using Tournament.Shared.DTOs;
using Tournament.Core.Responses;
using Tournament.Shared.Requests;

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