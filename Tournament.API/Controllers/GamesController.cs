using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Data;

namespace Tournament.Api.Controllers;

[Route("api/TournamentDetails/{TournamentDetailsId}/Games")]
//[Route("api/Games")]
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
        var torments =  (await _context.GameRepository.GetAllAsync()).AsQueryable().ProjectTo<GameDto>(_mapper.ConfigurationProvider).ToList();
        return Ok(torments);
    }


    // GET: api/TournamentDetails/1/Games
    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<Game>>> GetGame(int TournamentDetailsId)
    //{
    //    return await _context.Game.Where(g=>g.TournamentDetailsId.Equals(TournamentDetailsId)).ToListAsync();
    //    //return Ok(await _context.GameRepository.GetAllAsync());
    //}

    // GET: api/Games/5
    // GET: api/TournamentDetails/1/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(int id)
    {
        //var game = await _context.Game.SingleOrDefaultAsync(g => g.Id == id);
        var game = await _context.GameRepository.GetAsync(id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(int id, Game game)
    {
        if (id != game.Id)
        {
            return BadRequest();
        }

        //TODO
        //_context.Entry(game).State = EntityState.Modified;
        _context.GameRepository.SetStateModified(game);
        

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
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame(Game game)
    {
        //_context.Game.Add(game);
        //await _context.SaveChangesAsync();

        _context.GameRepository.Add(game);
        await _context.CompleteAsync();
 

        return CreatedAtAction("GetGame", new { id = game.Id,  TournamentDetailsId=game.TournamentDetailsId }, game);
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

        //await _context.SaveChangesAsync();
        await _context.CompleteAsync();

        return NoContent();
    }

    private  async Task<bool> GameExistsAsync(int id)
    {
        //return  _context.Game.Any(o=>o.Id == id);
        return await _context.GameRepository.AnyAsync(id);
    }
}
