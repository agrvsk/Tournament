using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Requests;
using Tournament.Core.Responses;
using Tournament.Data.Data;
using Tournaments.Presentation.Controllers;
using Tournaments.Services;

namespace Tournament.Api.Tests;

public class AuthControllerShould
{
    private readonly Mock<IServiceManager> ServiceManagerMock;
    private readonly Mapper Mapper;
    private readonly Mock<UserManager<User>> UserManager;
    private readonly AuthController Sut;

    public AuthControllerShould() 
    {
        ServiceManagerMock = new Mock<IServiceManager>();
        Sut = new AuthController(ServiceManagerMock.Object); //, Mapper, UserManager.Object

        Mapper = new Mapper(new MapperConfiguration(cfg => { cfg.AddProfile<TournamentMappings>(); }));
        var mockUserStore = new Mock<IUserStore<User>>();
        UserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
    }

    public static IEnumerable<object[]> GetIR()
    {
        yield return new object[] { true,  IdentityResult.Success };
        yield return new object[] { false, IdentityResult.Failed(new IdentityError { Description = "Role does not exist" }) };
    }

    [Theory]
    [MemberData(nameof(GetIR))]
    [Trait("HEJ","SVEJS")]
    public async Task RegisterUser(bool Ok, IdentityResult ir)
    {
        UserForRegistrationDto dto=new UserForRegistrationDto {Name="", Password="", UserName="", Email="", Role="" };
        ServiceManagerMock.Setup(o => o.AuthService.RegisterUserAsync(It.IsAny<UserForRegistrationDto>())).ReturnsAsync( ir );

        //ArgumentNullException  (dto)
        //IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });

        var sut = Sut;
        var res = await sut.RegisterUser(dto);

        if (Ok)
        {
            var test = Assert.IsType<StatusCodeResult>(res);
            Assert.Equal(201, test.StatusCode);
        }
        else
        {
            var test = Assert.IsType<BadRequestObjectResult>(res);
            var text = Assert.IsAssignableFrom<IEnumerable<IdentityError>>(test.Value);
        }

    }
    public async Task Authenticate()
    {
        //UserForAuthDto userForAuthDto

        //if (!await serviceManager.AuthService.ValidateUserAsync(userForAuthDto))
        //{
        //    return Unauthorized();
        //}

        //// var token = new { Token = await serviceManager.AuthService.CreateTokenAsync() };
        //TokenDto token = await serviceManager.AuthService.CreateTokenAsync(expireTime: true);

        //return Ok(token);

    }

    /*  AuthService:
        Task<TokenDto> CreateTokenAsync(bool expireTime);
        Task<ApiBaseResponse> GetAllUsersAsync(UserRequestParams uParams);
        Task<TokenDto> RefreshTokenAsync(TokenDto token);
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto registrationDto);
        Task<bool> ValidateUserAsync(UserForAuthDto userForAuthDto);
     */
    [Fact]
    public async Task GetAllUsers_IfAuth_ShouldReturn200Ok()
    {
        //Arrange
        var users = GetUsers();
        var dtos = Mapper.Map<IEnumerable<UserForRegistrationDto>>(users);
        ApiBaseResponse baseResponse = new ApiOkResponse<IEnumerable<UserForRegistrationDto>>(dtos);
        ServiceManagerMock.Setup(o => o.AuthService.GetAllUsersAsync(It.IsAny<UserRequestParams>()) ).ReturnsAsync(baseResponse);

        UserRequestParams uq = new UserRequestParams();
        var httpContextMock = new Mock<HttpContext>();

        httpContextMock.Setup(x => x.User.Identity.IsAuthenticated).Returns(true);

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContextMock.Object
        };
        var sut = Sut;
        sut.ControllerContext = controllerContext;

        var res = await sut.GetAllUsers(uq);

        var resultType = res.Result as OkObjectResult;

        var obj = Assert.IsType<OkObjectResult>(resultType);
        var xxx = Assert.IsAssignableFrom<IEnumerable<UserForRegistrationDto>>(resultType.Value);
        Assert.Equal(users.Count, xxx.Count());
        Assert.Contains<UserForRegistrationDto>(dtos.FirstOrDefault(), xxx); 
        //        Assert.Equal("Is not auth", resultType.Value);

    }
    

    [Fact]
    public async Task GetAllUsers_IfNotAuth_ShouldReturn400BadRequest()
    {
        var httpContextMock = new Mock<HttpContext>();
        UserRequestParams uq = new UserRequestParams();

        httpContextMock.SetupGet(x => x.User.Identity.IsAuthenticated).Returns(false);

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContextMock.Object
        };

        var sut = Sut;
        sut.ControllerContext = controllerContext;

        var res = await sut.GetAllUsers(uq);

        var resultType = res.Result as BadRequestObjectResult;

        Assert.IsType<BadRequestObjectResult>(resultType);
        Assert.Equal("Is not auth", resultType.Value);
    }




    public List<User> GetUsers()
    {
        return new List<User>
        {
            new User
            {
                 Id = "1",
                 //Name = "Kalle",
                 //Age = 12,
                 UserName = "Kalle"
            },
           new User
            {
                 Id = "2",
                 //Name = "Kalle",
                 //Age = 12,
                 UserName = "Kalle"
            },
        };
    }
}
