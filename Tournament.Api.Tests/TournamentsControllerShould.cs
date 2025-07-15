using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.IO;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MockQueryable.Moq;
using Moq;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Exceptions;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using Tournament.Core.Responses;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournaments.Presentation.Controllers;
using Tournaments.Services;
using Xunit.Abstractions;
using static Tournaments.Presentation.Controllers.TournamentDetailsController;

namespace Tournament.Api.Tests;

public class TournamentsControllerShould
{
    private TournamentDetailsController sut;
    private Mock<IServiceManager> MoqServiceManager;
    private Mock<ITournamentDetailsService> MockService;
    private TournamentDetailsService RealService;
    private Mock<ITournamentUoW> MoqUow;
    private Mock<ITournamentRepository> MoqRepo;

    public TournamentsControllerShould()
    {
        //---------------------------------------------------------------
        //Repository-Layer
        //---------------------------------------------------------------
        MoqRepo = new Mock<ITournamentRepository>();
        MoqUow = new Mock<ITournamentUoW>();
            MoqUow.Setup(o => o.TournamentRepository).Returns(MoqRepo.Object);

        //---------------------------------------------------------------
        //Service-Layer
        //---------------------------------------------------------------
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TournamentMappings>();
        }));

        RealService = new TournamentDetailsService(MoqUow.Object, mapper);
        MockService = new Mock<ITournamentDetailsService>();

        MoqServiceManager = new Mock<IServiceManager>();

        //---------------------------------------------------------------
        //Controller-Layer
        //---------------------------------------------------------------
        sut = new TournamentDetailsController(MoqServiceManager.Object);

        //-----------------------------------------
        // Mock the ObjectValidator
        //-----------------------------------------
        var mockValidator = new Mock<IObjectModelValidator>();
        mockValidator.Setup(v => v.Validate(
            It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<object>()));
        sut.ObjectValidator = mockValidator.Object;
        //-----------------------------------------
        //Mock the HttpContext
        //-----------------------------------------
        //var httpContextMock = new Mock<HttpContext>();
        //httpContextMock.SetupGet(x => x.User.Identity.IsAuthenticated).Returns(false);

        //-----------------------------------------
        //Use DefaultHttpContext
        //-----------------------------------------
        var controllerContext = new ControllerContext()
        {
//            HttpContext = httpContextMock.Object
            HttpContext = new DefaultHttpContext()
        };
        //---------------------------------------------------------------
        sut.ControllerContext = controllerContext;

    }
    /*
    Task<ApiBaseResponse> GetAllAsync(TournamentRequestParams tParams);
    Task<ApiBaseResponse> GetAsync(int id, bool showGames = false);
    Task<ApiBaseResponse> CreateAsync(TournamentCreateDto create);
    Task<ApiBaseResponse> UpdateAsync(TournamentUpdateDto update);
    Task<ApiBaseResponse> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument);
    Task<ApiBaseResponse> DeleteAsync(int id);
    */

    [Fact]
    public async Task GetTournamentDetails_ShouldReturnAllTournaments()
    {
        //Arrange - Mock Service
        //MoqServiceManager.Setup(o => o.TournamentService).Returns(MockService.Object);
        //ApiBaseResponse resp = TestResponse(TournamentDtos4Test);
        //MoqService.Setup(x => x.GetAllAsync(It.IsAny<TournamentRequestParams>())).ReturnsAsync( resp );

        //Arrange - Real Service
        MoqServiceManager.Setup(o => o.TournamentService).Returns(RealService);
        PagedList<TournamentDetails> tourn = Paged(Tournaments4Test, 1, 20);
        MoqRepo.Setup(x => x.GetAllAsync(It.IsAny<TournamentRequestParams>())).ReturnsAsync(tourn);

        //In-parameter
        TournamentRequestParams paams = new TournamentRequestParams { PageNumber = 1, PageSize = 20, ShowGames = false, Sort = false };
        
        //Act
        var result = await sut.GetTournamentDetails(paams);

        // Asserts
        var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
        //var items = Assert.IsType<IEnumerable<TournamentDto>>(okObjectResult.Value);
        var items = Assert.IsAssignableFrom<IEnumerable<TournamentDto>>(okObjectResult.Value);
        Assert.Equal(Tournaments4Test.Count(), items.Count()  );
    }

    [Theory]
    [InlineData(1,  true)]     //Ok
    [InlineData(2,  true)]     //Ok
    [InlineData(3,  false)]    //Data not found - TournamentNotFoundException()
    public async Task GetTournamentDetails_ShouldReturnTournament(int Id, bool exp)
    {
        //Arrange - Real Service
        MoqServiceManager.Setup(o => o.TournamentService).Returns(RealService);
        TournamentDetails x = Tournaments4Test.Where(x => x.Id == Id).SingleOrDefault();
        MoqRepo.Setup(x => x.GetAsync(It.IsAny<int>(), false)).ReturnsAsync(x);


        // Asserts
        if (exp)
        {
            //Act
            var result = await sut.GetTournamentDetails(Id, showGames: false);

            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var obj = Assert.IsType<TournamentDto>(okObjectResult.Value);
            Assert.Equal(x.Title, obj.Title);
            //var okObjectResult = Assert.IsAssignableFrom<ApiBaseResponse>(result.Result);
            //var items = Assert.IsType<IEnumerable<TournamentDto>>(okObjectResult.Value);
            //var items = Assert.IsAssignableFrom<IEnumerable<TournamentDto>>(okObjectResult.Value);
            //Assert.Equal(Tournaments4Test.Count(), items.Count());
        }
        else
        {
            //Act
            var result = await Assert.ThrowsAsync<TournamentNotFoundException>(() => sut.GetTournamentDetails(Id, showGames: false));
        }
    }

    [Theory]
    [InlineData(1, 3, "Titel 1", false)]    //Dto invalid
    [InlineData(1, 1, "Titel 1", true)]     //Ok
    [InlineData(3, 3, "Titel 1", false)]    //Data not found - TournamentNotFoundException()

    public async Task PutTest1Async(int Id, int DtoId, string DtoTitle, bool expectedOk)
    {
        //Arrange - Real Service
        MoqServiceManager.Setup(o => o.TournamentService).Returns(RealService);
        TournamentDetails x = Tournaments4Test.Where(x => x.Id == Id).SingleOrDefault();
        MoqRepo.Setup(x => x.GetAsync(It.IsAny<int>(), false )).ReturnsAsync(x);


        //TournamentDetails x = new TournamentDetails { Id = 1, Title = "Title 1", StartDate = DateTime.Now, Games = new List<Game>() };
        //MoqRepo.Setup(u => u.GetAsync(1, false)).Returns(Task.FromResult(x));

        //        MoqRepo.Setup(m => m.Method(It.IsAny<string>())).Returns("Default");
        //        MoqRepo.Setup(m => m.Method("specific")).Returns("SpecificResult");

        //        MoqRepo.SetupSequence(m => m.Method())
        //    .Returns("First")
        //    .Returns("Second")
        //    .Throws(new Exception("Third call fails"));
        
        //MoqRepo.Setup(m => m.MethodA()).Returns(42);
        //MoqRepo.Setup(m => m.MethodB("hello")).Returns(true);
        //MoqRepo.Setup(m => m.MethodC(It.IsAny<int>())).Throws<InvalidOperationException>();


        //Arrange Dto för Update skapas utifrån Theory Params
        var dto = new TournamentUpdateDto { Id = DtoId, Title = DtoTitle, StartDate = DateTime.Now };

        //Testa Validering för modelbinding
        //var context = new ValidationContext(dto);
        //var results = new List<ValidationResult>();
        //var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);

        //Act
        if (Id > 2)
        {
            var ex = await Assert.ThrowsAsync<TournamentNotFoundException>(() => sut.PutTournamentDetails(Id, dto));
            Assert.NotNull(ex);
            Assert.IsType<TournamentNotFoundException>(ex);
            Assert.Equal("The tournament with id 3 is not found", ex.Message);

            return;
        }


         var retur = await sut.PutTournamentDetails(Id, dto);
        //var retur = await tdc.PutTournamentDetails(Id, dto);

        //var ex = Assert.Throws<ArgumentException>(() => service.DoSomething(null));
        //Assert.Equal("Input cannot be null", ex.Message);


        //Assert
        if (expectedOk)
        {
            Assert.IsType<NoContentResult>(retur);
        }
        else
        {
            Assert.IsType<BadRequestResult>(retur);
        }

    }
    //-----------------------------------------------------------
    //static private PagedList<T> UserList<T>(IEnumerable<T> data, int PageNumber, int PageSize)
    //{
    //    var x = new PagedList<TournamentDetails>(Users, 2, 1, 20);
    //    return PagedList<T>.CreateAsync(data.AsQueryable<T>(), PageNumber, PageSize);
    //}
    //static private Task<PagedList<TournamentDetails>> Paged(List<TournamentDetails> data, int pageNr = 1, int pageSize = 20)
    static private PagedList<TournamentDetails> Paged(IEnumerable<TournamentDetails> data, int pageNr = 1, int pageSize = 20)
    //  static private Task<PagedList<TournamentDetails>> Paged(IEnumerable<TournamentDetails> data, int pageNr=1, int pageSize=20)
    {
        //        var y = (IQueryable<TournamentDetails>)data;
        //        var y = data.AsQueryable<TournamentDetails>();
        //var y = data.AsQueryable();

        //       return PagedList<TournamentDetails>.CreateAsync(y, pageNr, pageSize);

        //        var x = new PagedList<IEnumerable<TournamentDetails>>(data, 2, pageNr, pageSize);
        //return new PagedList<IEnumerable<TournamentDetails>>(data, 2, pageNr, pageSize);

        //        return new Tournament.Core.Requests.PagedList(data, 2, pageNr, pageSize);
        return new PagedList<TournamentDetails>(data, 2, pageNr, pageSize);
    }
    //-------------------------------------------------------
    static private ApiBaseResponse TestResponse(IEnumerable<TournamentDto> TournamentDtos)
    {
        return new ApiOkResponse<IEnumerable<TournamentDto>>(TournamentDtos);
    }
    static private IEnumerable<TournamentDto> TournamentDtos4Test
    {
        get 
        {
            IEnumerable <TournamentDto> x =
            [
                new TournamentDto ( Title: "Title 1", StartDate: DateTime.Now ),
                new TournamentDto ( Title: "Title 2", StartDate: DateTime.Now )
            ];
            return x;
        }
    }
    //-------------------------------------------------------


    //static private List<TournamentDetails> Tournaments4Test
    static private IEnumerable<TournamentDetails> Tournaments4Test
    {
        get
        {
            IEnumerable<TournamentDetails> x =
            [
            //return new List<TournamentDetails>
            //{
                new TournamentDetails { Id = 1, Title = "Title 1", StartDate = DateTime.Now, Games = new List<Game>() },
                new TournamentDetails { Id = 2, Title = "Title 2", StartDate = DateTime.Now, Games = new List<Game>() }
            //};
            ];
            return x;
        }
    }


    static private TournamentDetailsController XXXTournamentDetailsController
    {
        get
        {
            //IEnumerable<TournamentDetails> x =
            //[
            //    new TournamentDetails { Id = 1, Title = "Title 1", StartDate = DateTime.Now, Games = new List<Game>() },
            //    new TournamentDetails { Id = 2, Title = "Title 2", StartDate = DateTime.Now, Games = new List<Game>() }
            //];

            TournamentDetails x = new TournamentDetails { Id = 1, Title = "Title 1", StartDate = DateTime.Now, Games = new List<Game>() };


            //_context.TournamentRepository.GetAllAsync(fi.ShowGames, fi.Sort)


            var UoW = new Mock<ITournamentUoW>();
    //      UoW.Setup(u => u.TournamentRepository.GetAllAsync(false, true))
            UoW.Setup(u => u.TournamentRepository.GetAsync(1, false))
                .Returns(Task.FromResult(x
                 ));


            var map = new Mock<IMapper>();

            map.Setup(m => m.Map<TournamentUpdateDto, TournamentDetails>
                       (It.IsAny<TournamentUpdateDto>()))
                .Returns((TournamentUpdateDto src) => new TournamentDetails
                {
                    Id = 1,
                    Title = "Title 1",
                    StartDate = DateTime.Now,
                    Games = new List<Game>()
                });


            //map.Setup(m => m.Map<TournamentDetails>(It.IsAny<TournamentUpdateDto>())) //, It.IsAny<TournamentDetails>()
            //   .Returns(value: new TournamentDetails() { Id=1, Title="MyCup", StartDate=DateTime.Now } );

            //map.Setup(m => m.Map<TournamentDto>(It.IsAny<TournamentDetails>()))
            //          .Returns(value: new TournamentDto("MyCup", DateTime.Now));


            // Mock the ObjectValidator
            var mockValidator = new Mock<IObjectModelValidator>();
            mockValidator.Setup(v => v.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<object>()));


            var lTour = new Lazy<ITournamentDetailsService>(() => new TournamentDetailsService(UoW.Object, map.Object));
            var lGame = new Lazy<IGameService>(() => new GameService(UoW.Object, map.Object));


            //(IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)

            //var lAuth = new Lazy<IAuthService>(() => new AuthService(map.Object, new UserManager<User> x, new RoleManager role, IConfiguration config));
            Lazy<IAuthService> lAuth = null;
            ServiceManager m = new ServiceManager(lTour, lGame, lAuth);

            TournamentDetailsController controller = new TournamentDetailsController(m);
            controller.ObjectValidator = mockValidator.Object;
            return controller;

                //new TournamentDetailsController(UoW.Object, map.Object);
        }
    }
}