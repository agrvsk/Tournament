using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Bogus.DataSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Api.Controllers;

[Route("api/TournamentDetails")]
[ApiController]
public class TournamentDetailsController : ControllerBase
{
    private readonly TournamentContext _context;
    private readonly IMapper _mapper;

    public TournamentDetailsController(TournamentContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails()
    {
        //return await _context.TournamentDetails.ToListAsync();
        var torments = await _context.TournamentDetails.ProjectTo<TournamentDto>(_mapper.ConfigurationProvider).ToListAsync();
        return Ok(torments);
    }

    // GET: api/TournamentDetails/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
    {
        var tournamentDetails = await _context.TournamentDetails.FindAsync(id);

        if (tournamentDetails == null)
        {
            return NotFound();
        }
        var dto = _mapper.Map<TournamentDto>(tournamentDetails);
        return Ok(dto);
    }

    // PUT: api/TournamentDetails/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTournamentDetails(int id, TournamentDetails tournamentDetails)
    {
        if (id != tournamentDetails.Id)
        {
            return BadRequest();
        }

        _context.Entry(tournamentDetails).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TournamentDetailsExists(id))
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

    // POST: api/TournamentDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TournamentDto>> PostTournamentDetails(TornamentCreateDto dto)
    {
        var torment = _mapper.Map<TournamentDetails>(dto);
        _context.TournamentDetails.Add(torment);
        await _context.SaveChangesAsync();
        var createdTorment = _mapper.Map<TournamentDto>(torment);

        //_context.TournamentDetails.Add(tournamentDetails);
        //await _context.SaveChangesAsync();

//      return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetails.Id }, tournamentDetails);
        return CreatedAtAction(nameof(GetTournamentDetails), new { id = torment.Id }, createdTorment);

    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
        if (tournamentDetails == null)
        {
            return NotFound();
        }

        _context.TournamentDetails.Remove(tournamentDetails);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TournamentDetailsExists(int id)
    {
        return _context.TournamentDetails.Any(e => e.Id == id);
    }
}
