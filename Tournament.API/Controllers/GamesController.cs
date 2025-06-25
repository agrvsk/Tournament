using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus.DataSets;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Api.Controllers;

//[Route("api/TournamentDetails/{TournamentDetailsId}/Games")]
[Route("api/Games")]
[ApiController]
public class GamesController(ITournamentUoW _context, IMapper _mapper) : ControllerBase
{
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
    // GET: api/TournamentDetails/1/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGame()
    {
        //return await _context.Game.ToListAsync();
        //return Ok(await _context.GameRepository.GetAllAsync());
        var allGames = await _context.GameRepository.GetAllAsync();
        if (allGames == null || allGames.IsNullOrEmpty()) return NotFound();

        //var dtos = allGames.AsQueryable().ProjectTo<GameDto>(_mapper.ConfigurationProvider).ToList();
        var dtos = _mapper.Map<IEnumerable<GameDto>>(allGames);
        return Ok(dtos);
    }


    // GET: api/Games/5
    // GET: api/TournamentDetails/1/Games/5
    [HttpGet("{title}")]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetGames(string title)
    {
        var game = await _context.GameRepository.GetByTitleAsync(title);

        if (game == null)
        {
            return NotFound();
        }
        var dtos = _mapper.Map<IEnumerable<GameDto>>(game);
        return Ok(dtos);
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<GameDto>> GetGame(int id)
    //{
    //    //var game = await _context.Game.SingleOrDefaultAsync(g => g.Id == id);
    //    var game = await _context.GameRepository.GetAsync(id);
    //    if (game == null)
    //    {
    //        return NotFound();
    //    }
    //    var dto = _mapper.Map<GameDto>(game);
    //    return Ok(dto);
    //}

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

        var gameExist = await _context.GameRepository.GetAsync(id);
        if(gameExist == null) return NotFound();

        _mapper.Map(dto, gameExist);

        //När Behövs följande????
        //_context.Entry(game).State = EntityState.Modified;
        _context.GameRepository.SetStateModified(gameExist);
        

        try
        {
//          await _context.SaveChangesAsync();
            await _context.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await GameExistsAsync(id))
            {
                return NotFound();
            }
            else
            {
                //throw;
                return StatusCode(500);
            }
        }
        //var createdGame = _mapper.Map<GameDto>(gameExist);
        return NoContent();
        //return CreatedAtAction(nameof(GetGame), new { id = gameExist.Id, TournamentDetailsId = gameExist.TournamentDetailsId }, createdGame);

    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame(GameCreateDto dto)
    {
        TryValidateModel(dto);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        //_context.Game.Add(game);
        //await _context.SaveChangesAsync();
        var game = _mapper.Map<Game>(dto);

        _context.GameRepository.Add(game);

        try
        {
            //await _context.SaveChangesAsync();
            await _context.CompleteAsync();
        }
        catch (DBConcurrencyException)
        {
            //throw;
            return StatusCode(500);
        }
        var createdGame = _mapper.Map<GameDto>(game);

        //return CreatedAtAction(nameof(GetGame), new { id = game.Id,  TournamentDetailsId=game.TournamentDetailsId }, game);
        return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
//        var game = await _context.Game.FindAsync(id);
        var game = await _context.GameRepository.GetAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        //_context.Game.Remove(game);
        _context.GameRepository.Remove(game);

        try
        {
            //await _context.SaveChangesAsync();
            await _context.CompleteAsync();
        }
        catch (DBConcurrencyException)
        {
            //throw;
            return StatusCode(500);
        }

        return NoContent();
    }

    private  async Task<bool> GameExistsAsync(int id)
    {
        //return  _context.Game.Any(o=>o.Id == id);
        return await _context.GameRepository.AnyAsync(id);
    }
}
