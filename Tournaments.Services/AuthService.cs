using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournaments.Services;

public class AuthService : IAuthService
{
    private readonly IMapper mapper;
    private readonly ITournamentUoW _uow;
    private readonly UserManager<User> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration config;
    private User? user;

    public AuthService(IMapper mapper, ITournamentUoW uow, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
    {
        this.mapper = mapper;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.config = config;
        _uow = uow;
    }


    //public async Task<string> CreateTokenAsync()
    //{
    //    SigningCredentials signing = GetSigningCredentials();
    //    IEnumerable<Claim> claims = await GetClaimsAsync();
    //    JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims);

    //    return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    //}

    public async Task<TokenDto> CreateTokenAsync(bool expireTime)
    {
        ArgumentNullException.ThrowIfNull(nameof(user));
        SigningCredentials signing = GetSigningCredentials();
        IEnumerable<Claim> claims = await GetClaimsAsync();
        JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims);

        user!.RefreshToken = GenerateRefreshToken();

        if(expireTime)
        { user.Expires = DateTime.UtcNow.AddDays(7); }

        var res = await userManager.UpdateAsync(user);
        // Validera res

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, user.RefreshToken!);
    }

    private string? GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signing, IEnumerable<Claim> claims)
    {
        var jwtSettings = config.GetSection("JwtSettings");

        var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["Expires"])),
                signingCredentials: signing
            );
        return tokenOptions;
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync()
    {
        ArgumentNullException.ThrowIfNull(nameof(user));
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.UserName),
        //  new Claim("Age", user.Age.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;

    }

    private SigningCredentials GetSigningCredentials()
    {
        var secretKey = config["secretkey"];
        ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }






    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto registrationDto)
    {
        ArgumentNullException.ThrowIfNull(registrationDto);

        var roleExists = await roleManager.RoleExistsAsync(registrationDto.Role);
        if (!roleExists)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
        }

        var user = mapper.Map<User>(registrationDto);

        var result = await userManager.CreateAsync(user, registrationDto.Password!);

        if(result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, registrationDto.Role);
        }

        return result;
    }

    public async Task<bool> ValidateUserAsync(UserForAuthDto userForAuthDto)
    {
        if (userForAuthDto is null)
        {
            throw new ArgumentNullException(nameof(userForAuthDto));
        }

        user = await userManager.FindByNameAsync(userForAuthDto.UserName);
        return user != null && await userManager.CheckPasswordAsync(user, userForAuthDto.PassWord);
    }

    public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);

        User? user = await userManager.FindByNameAsync(principal.Identity?.Name!);

        if (user == null || user.RefreshToken != token.RefreshToken || user.Expires <= DateTime.UtcNow)
        {
            throw new ArgumentException("The TokenDto has some invalid values");
        }

        this.user = user;

        return await CreateTokenAsync(expireTime: false);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        ArgumentNullException.ThrowIfNull(nameof(jwtSettings));

        var secretKey = config["secretkey"];
        ArgumentNullException.ThrowIfNull(nameof(secretKey));

        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // Ändras till false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParams, out SecurityToken securityToken);

        if(securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid Token");
        }

        return principal;

    }

    public async Task<ResultObjectDto<IEnumerable<UserForRegistrationDto>>> GetAllUsersAsync()
    {
        ResultObjectDto<IEnumerable<UserForRegistrationDto>> retur = new ResultObjectDto<IEnumerable<UserForRegistrationDto>>();
        retur.Message = string.Empty;
        retur.IsSuccess = true;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 200;

        var objects = await _uow.UserRepository.GetUsersAsync();  //AllAsync(sorted, pageNr, pageSize);

        IEnumerable<UserForRegistrationDto> dtos = mapper.Map<IEnumerable<UserForRegistrationDto>>(userManager.Users.ToList<User>());
        //await userManager.Users.ToListAsync();
        retur.Data = dtos;


        return retur;
    }
}

