using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using AutoMapper;
//using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Tournament.Shared.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Responses;
using Tournament.Shared.Requests;

namespace Tournaments.Presentation.Controllers;

//[Route("api/TournamentDetails/{TournamentDetailsId}/Games")]
[Route("api/Games")]
[ApiController]
public class GamesController(IServiceManager _serviceManager) : ApiControllerBase
{
    readonly int maxGamesPerPage = 100;
    //private readonly TournamentContext _context;
    //private readonly ITournamentUoW _context;
    //private readonly IMapper _mapper;
    //public GamesController(ITournamentUoW context, IMapper mapper)
    //{
    //    _context = context;
    //    _mapper = mapper;
    //}
    //public GamesController(TournamentContext context, IMapper mapper)
    //{
    //    _context = context;
    //    _mapper = mapper;
    //}


    // GET: api/Games
    [HttpGet]
  //public async Task<ActionResult<IEnumerable<GameDto>>> GetGame(bool sort, int pageNr = 1, int pageSize = 20)
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGame([FromQuery]GameRequestParams gParams)
    {

        //return await _context.Game.ToListAsync();
        //return Ok(await _context.GameRepository.GetAllAsync());
        if (gParams.PageSize > maxGamesPerPage)
            gParams.PageSize = maxGamesPerPage;

        var response = await _serviceManager.GameService.GetAllAsync(gParams);
        if (response.Success)
        {
            Add2Header(response.Paginering);
            return Ok(response.GetOkResult<IEnumerable<GameDto>>());
//            return response.Success ? Ok(response.GetOkResult<IEnumerable<GameDto>>()) : ProcessError(response);

        }
        else
        {
            return ProcessError(response);
        }

        //if (result.IsSuccess)
        //{
        //    if (result.Pagination != null)
        //        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.Pagination));

        //    return Ok(result.Data);
        //}
        //else
        //{
        //    return StatusCode(result.StatusCode);
        //}
            //var (allGames, pagination) = await _context.GameRepository.GetAllAsync(sort,pageNr,pageSize);
            //if (
            //  allGames == null || 
            //    allGames.IsNullOrEmpty()) return NotFound();
            //if( allGames == null ) return NotFound();

            //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

        //var dtos = allGames.AsQueryable().ProjectTo<GameDto>(_mapper.ConfigurationProvider).ToList();
        //var dtos = _mapper.Map<IEnumerable<GameDto>>(allGames);
        //return Ok(dtos);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGame(int id)
    {
        //var game = await _context.Game.SingleOrDefaultAsync(g => g.Id == id);
        var result=await _serviceManager.GameService.GetAsync(id);
        if (result.Success)
        {
            return Ok(result.GetOkResult<GameDto>());
        }
        else
        {
            return ProcessError(result);
        }
        //var game = await _context.GameRepository.GetAsync(id);
        //if (game == null)
        //{
        //    return NotFound();
        //}
        //var dto = _mapper.Map<GameDto>(game);
        //return Ok(dto);
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, GameUpdateDto dto)
    {
        if (id != dto.Id)
        {
            return BadRequest();
        }

        TryValidateModel(dto);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        var retur = await _serviceManager.GameService.UpdateAsync(dto);
        if (retur.Success)
        {
            retur.GetOkResult<GameUpdateDto>();
            return NoContent();
        }
        else
        {
            return ProcessError(retur);
        }

//        var gameExist = await _context.GameRepository.GetAsync(id);
//        if(gameExist == null) return NotFound();

//        _mapper.Map(dto, gameExist);

//        //När Behövs följande????
//        //_context.Entry(game).State = EntityState.Modified;
//        _context.GameRepository.SetStateModified(gameExist);
        

//        try
//        {
////          await _context.SaveChangesAsync();
//            await _context.CompleteAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!await GameExistsAsync(id))
//            {
//                return NotFound();
//            }
//            else
//            {
//                //throw;
//                return StatusCode(500);
//            }
//        }
        
//        return NoContent();

        //var createdGame = _mapper.Map<GameDto>(gameExist);
        //return CreatedAtAction(nameof(GetGame), new { id = gameExist.Id, TournamentDetailsId = gameExist.TournamentDetailsId }, createdGame);

    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    //[Route("/api/TournamentDetails/{TournamentDetailsId}/Games")]
    public async Task<ActionResult<GameUpdateDto>> PostGame( GameCreateDto dto)
    {
        TryValidateModel(dto);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

//        var tourment = await _context.TournamentRepository.GetAsync(dto.TournamentDetailsId);
//        if(tourment == null) 
//            return NotFound($"Tournament with Id {dto.TournamentDetailsId} was not found.");

        //_context.Game.Add(game);
        //await _context.SaveChangesAsync();
        //var game = _mapper.Map<Game>(dto);

        //_context.GameRepository.Add(game);

        //try
        //{
        //    //await _context.SaveChangesAsync();
        //    await _context.CompleteAsync();
        //}
        //catch (DBConcurrencyException)
        //{
        //    //throw;
        //    return StatusCode(500);
        //}
        //var createdGame = _mapper.Map<GameDto>(game);


        var retur = await _serviceManager.GameService.CreateAsync(dto);
        if (retur.Success)
        {
            var u_dto = retur.GetOkResult<GameUpdateDto>();
            return CreatedAtAction(nameof(GetGame), new { id = u_dto.Id }, retur.GetOkResult<GameUpdateDto>() );
        }
        else
        {
            return ProcessError(retur);
        }
        //return CreatedAtAction(nameof(GetGame), new { id = game.Id,  TournamentDetailsId=game.TournamentDetailsId }, game);
        //return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var result = await _serviceManager.GameService.DeleteAsync(id);
        if (result.Success)
        {
            return NoContent();
        }
        else
            return ProcessError(result); 

        //    //        var game = await _context.Game.FindAsync(id);
        //    var game = await _context.GameRepository.GetAsync(id);

        //if (game == null)
        //{
        //    return NotFound();
        //}

        ////_context.Game.Remove(game);
        //_context.GameRepository.Remove(game);

        //try
        //{
        //    //await _context.SaveChangesAsync();
        //    await _context.CompleteAsync();
        //}
        //catch (DBConcurrencyException)
        //{
        //    //throw;
        //    return StatusCode(500);
        //}

        //return NoContent();
    }

    //private  async Task<bool> GameExistsAsync(int id)
    //{
    //    //return  _context.Game.Any(o=>o.Id == id);
    //    return await _context.GameRepository.AnyAsync(id);
    //}
}
