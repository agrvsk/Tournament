using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.Contracts;
using Services.Contracts;
using Tournament.Api.Controllers;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournaments.Services;
using static Tournament.Api.Controllers.TournamentDetailsController;

namespace Tournament.Api.Tests;

public class TournamentsShould
{
    //[Fact]
    [Theory]
    [InlineData(1, 3, "Titel 1", false)]
    [InlineData(1, 1, "Titel 1", true)]
    public async Task PutTest1Async(int Id, int DtoId, string DtoTitle, bool expectedOk)
    {
        //Arrange
        TournamentDetailsController tdc = TournamentDetailsController;
        var dto = new TournamentUpdateDto { Id = DtoId, Title = DtoTitle, StartDate = DateTime.Now };

        //Testa Validering för modelbinding
        //var context = new ValidationContext(dto);
        //var results = new List<ValidationResult>();
        //var isValid = Validator.TryValidateObject(dto, context, results, validateAllProperties: true);


        //Act
        var retur = await tdc.PutTournamentDetails(Id, dto);

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

    static private TournamentDetailsController TournamentDetailsController
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