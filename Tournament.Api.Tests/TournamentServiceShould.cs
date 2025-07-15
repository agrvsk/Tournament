using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Exceptions;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using Tournament.Core.Responses;
using Tournament.Data.Data;
using Tournaments.Services;


namespace Tournament.Api.Tests;

public class TournamentServiceShould
{
    public Mock<ITournamentRepository> MoqRepo;
    public Mock<ITournamentUoW> MoqUow;
    public TournamentDetailsService sut;

    public TournamentServiceShould()
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
        sut = new TournamentDetailsService(MoqUow.Object, mapper);
    }
    [Fact]
    public async Task GetAllAsync_shouldReturnAll()
    {
        //In-parameter
        TournamentRequestParams paams = new TournamentRequestParams { PageNumber = 1, PageSize = 20, ShowGames = false, Sort = false };

        PagedList<TournamentDetails> page = Paged(Tournaments4Test, 1, 20);
        MoqRepo.Setup(x => x.GetAllAsync(It.IsAny<TournamentRequestParams>()))
            .ReturnsAsync(page);

        var result = await sut.GetAllAsync(paams);

        // Asserta
        Assert.NotNull(result);
        var okObjectResult = Assert.IsType<ApiOkResponse<IEnumerable<TournamentDto>>>(result);
        var items = okObjectResult?.Result;
        Assert.Equal(Tournaments4Test.Count(), items.Count() );
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public async Task GetAsync_should_One_If_Found(int Id, bool Ok)
    {
        var v = Tournaments4Test.Where(o => o.Id == Id).SingleOrDefault();
        MoqRepo.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<bool>()))
        .ReturnsAsync(v);

        if (Ok)
        {
            var result = await sut.GetAsync(Id, false);

            // Asserta
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<ApiOkResponse<TournamentDto>>(result);
            var data = okObjectResult.Result;
            Assert.Equal(v.Title, data.Title);

        }
        else
        {
            var result = await Assert.ThrowsAsync<TournamentNotFoundException>(() => sut.GetAsync(Id, showGames: false));
            Assert.Equal($"The tournament with id {Id} is not found", result.Message);
        }
    }

    [Theory]
    [InlineData("TESTARE", true)]
    [InlineData("" , false)]
    public async Task CreateAsync_Should(string? Title, bool Ok)
    {
        //var list = new List<TournamentDetails>();

        TournamentCreateDto dto = new TournamentCreateDto { Title = Title, StartDate = DateTime.Now };

        MoqRepo.Setup(x => x.Add(It.IsAny<TournamentDetails>()));
        //.ReturnsAsync(v);
        if (!Ok)
        {
            var msg = await Assert.ThrowsAsync<ValidationException>(() => sut.CreateAsync(dto));
            Assert.Equal("Title is a required field.", msg.Message);
            return;
        }
        var result = await sut.CreateAsync(dto);

        // Asserta
        Assert.NotNull(result);
        var okObjectResult = Assert.IsType<ApiOkResponse<TournamentUpdateDto>>(result);
        var data = okObjectResult.Result;
        Assert.Equal(dto.Title, data.Title);
    }

    [Theory]
    [InlineData( 1, "TESTARE", true,  null )]
    [InlineData( 1, ""       , true, null )] //Valideringen endast i Controllern!
    [InlineData( 3, "TESTARE", false, "TournamentNotFoundException" )]

    public async Task UpdateAsync_should(int dtoId, string title, bool Ok, string ex)
    {
        var v = Tournaments4Test.Where(o => o.Id == dtoId).SingleOrDefault();
        MoqRepo.Setup(x => x.Update(It.IsAny<TournamentDetails>()));
        MoqRepo.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<bool>()))
        .ReturnsAsync(v);

        TournamentUpdateDto dto = new TournamentUpdateDto { Id=dtoId, Title = title, StartDate = DateTime.Now };
        if (!Ok)
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(dto));
            Assert.Equal(ex, exception.GetType().Name);
        }
        else
        {
            var result = await sut.UpdateAsync(dto);
            
            // Asserta
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<ApiOkResponse<TournamentDto>>(result);
            var data = okObjectResult.Result;
            Assert.Equal(dto.Title, data.Title);


        }
    }
    [Theory]
    [InlineData( 1, "TESTARE", true, null)]
    [InlineData( 2, "TESTARE", true, null)]
    [InlineData( 1, ""       , false, "ValidationException")] 
    [InlineData( 3, "TESTARE", false, "TournamentNotFoundException")]

    public async Task UpdateAsync_Shuld(int Id, string newTitle, bool Ok, string ex )
    {
        var v = Tournaments4Test.Where(o => o.Id == Id).SingleOrDefault();
        MoqRepo.Setup(x => x.Update(It.IsAny<TournamentDetails>()));
        MoqRepo.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<bool>()))
        .ReturnsAsync(v);

        //Task<ApiBaseResponse> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument);
        //void Update(TournamentDetails tournament);
        JsonPatchDocument<TournamentUpdateDto> patch = new JsonPatchDocument<TournamentUpdateDto>();
        patch.Replace(t => t.Title  , value: newTitle);
        if (!Ok)
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(() => sut.UpdateAsync(Id, patch));
            Assert.Equal(ex, exception.GetType().Name);

        }
        else
        {
            var result = await sut.UpdateAsync(Id, patch);
            // Asserta
            Assert.NotNull(result);
            var okObjectResult = Assert.IsType<ApiOkResponse<TournamentUpdateDto>>(result);
            var data = okObjectResult.Result;
//            Assert.Equal(newTitle, data.Title);

        }

    }

    public async Task DeleteAsync_should()
    {
        //Task<ApiBaseResponse> DeleteAsync(int id);
        //void Remove(TournamentDetails tournament);

    }

    static private PagedList<TournamentDetails> Paged(IEnumerable<TournamentDetails> data, int pageNr = 1, int pageSize = 20)
    {
        return new PagedList<TournamentDetails>(data, 2, pageNr, pageSize);
    }

    static private IEnumerable<TournamentDetails> Tournaments4Test
    {
        get
        {
            IEnumerable<TournamentDetails> x =
            [
                new TournamentDetails { Id = 1, Title = "Title 1", StartDate = DateTime.Now, Games = new List<Game>() },
                new TournamentDetails { Id = 2, Title = "Title 2", StartDate = DateTime.Now, Games = new List<Game>() }
            ];
            return x;
        }
    }

    /*
    */

    /*
    Task<bool> AnyAsync(int id);
    public void SetStateModified(TournamentDetails tournament);
    */
}
