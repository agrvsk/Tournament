using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.XPath;
using AutoMapper;
using AutoMapper.QueryableExtensions;
//using Bogus.DataSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using static System.Net.WebRequestMethods;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

//using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournaments.Presentation.Controllers;

[Route("api/TournamentDetails")]
[ApiController]
public class TournamentDetailsController(IServiceManager _serviceManager) : ControllerBase  //, UserManager<User> _userManager
{
    readonly int maxToursPerPage = 100;
    //private readonly TournamentContext _context;
    //private readonly ITournamentUoW _context;
    //private readonly IMapper _mapper;

    //public TournamentDetailsController(ITournamentUoW context, IMapper mapper, IServiceManager serviceManager) 
    //{
    //    _context = context;
    //    _mapper = mapper;
    //}

    //public TournamentDetailsController(TournamentContext context, IMapper mapper)
    //{
    //    _context = context;
    //    _mapper = mapper;
    //}

    //public class FilterObject
    //{
    //    public bool ShowGames { get; set; }
    //    public bool Sort { get; set; }
    //    [DefaultValue(1)]
    //    public int PageNumber { get; set; } = 1;
    //    [DefaultValue(20)]
    //    public int PageSize { get; set; } = 20;
    //}
    // GET: api/TournamentDetails
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails([FromQuery] TournamentRequestParams fi)
  //public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool showGames, bool sort)
    {
        //var auth = User.Identity.IsAuthenticated;
        //var userName = _userManager.GetUserName(User);
        //var user = await _userManager.GetUserAsync(User);

        if (fi.PageSize > maxToursPerPage)
            fi.PageSize = maxToursPerPage;
        //return await _context.TournamentDetails.ToListAsync();
        //var torments = await _context.TournamentDetails.ProjectTo<TournamentDto>(_mapper.ConfigurationProvider).ToListAsync();
        //var torments = showGames ? await _context.TournamentRepository.GetAllAsync(showGames, sort)
        //var torments = await _context.TournamentRepository.GetAllAsync(fi.ShowGames, fi.Sort);
        //if (torments == null || torments.IsNullOrEmpty()) return NotFound();

        //var dto = torments.AsQueryable().ProjectTo<TournamentDto>(_mapper.ConfigurationProvider).ToList();
        //var dto = _mapper.Map<IEnumerable<TournamentDto>>(torments);

        //IEnumerable<TournamentDto> dtos
        var retur  = await _serviceManager.TournamentService.GetAllAsync(fi.ShowGames, fi.Sort, fi.PageNumber, fi.PageSize);
        if (retur.IsSuccess)
        {
            if (retur.Pagination != null)
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(retur.Pagination));
            return Ok(retur.Data);
        }
        else
        {
            if (retur.Data == null)
            {
                return NotFound(Wrap(retur));
            }
            return StatusCode(retur.StatusCode, Wrap(retur));
        }

        //if(dtos.IsNullOrEmpty()) return NotFound();
    }

    public ProblemDetails Wrap<T>(ResultObjectDto<T> retur)
    {
        return new ProblemDetails
        {
            Type = "application/json",
            Title = "Fel inträffade vid bearbetning.",
            Detail = retur.Message,
            Status = retur.StatusCode,
            Instance = HttpContext.Request.Path
        };
    }


    // GET: api/TournamentDetails/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id, bool showGames)
    {
        //    var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
        //var tournamentDetails = showGames ? await _context.TournamentRepository.GetAsync(id, true)
        //                                  : await _context.TournamentRepository.GetAsync(id);
        //var dto = _mapper.Map<TournamentDto>(tournamentDetails);

        //var dto = 
        var retur = await _serviceManager.TournamentService.GetAsync(id, showGames);
        if (!retur.IsSuccess) 
            return NotFound(Wrap(retur));

            return Ok(retur.Data);
    }

    // PUT: api/TournamentDetails/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    //Om jag vill visa den ändrade posten?
    //    public async Task<ActionResult<TournamentDto>> PutTournamentDetails(int id, TournamentUpdateDto dto)
    public async Task<IActionResult> PutTournamentDetails(int id, TournamentUpdateDto dto)
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
        var status = await _serviceManager.TournamentService.UpdateAsync(dto);
        if (status.IsSuccess)
        {
            return NoContent();
        }
        else
        {
            return StatusCode(status.StatusCode, Wrap(status));
        }

        //var tournamentExist = await _context.TournamentDetails.SingleOrDefaultAsync(t=>t.Id == id);
        //var tournamentExist = await _context.TournamentRepository.GetAsync(id);
        //if (tournamentExist == null) return NotFound();
        //_mapper.Map(dto, tournamentExist);

        ////var dto = await _serviceManager.TournamentService.GetAsync(id);
        ////if (dto == null) return NotFound();

        //När Behövs följande????
        //      _context.Entry(tournamentExist).State = EntityState.Modified;
        //försök eftersom ovanstående ej finns i UoW
        //_context.TournamentRepository.SetStateModified(tournamentExist);


        //try
        //{
        //    //await _context.SaveChangesAsync();
        //    await _context.CompleteAsync();
        //}
        //catch (DbUpdateConcurrencyException)
        //{
        //    if (!await TournamentDetailsExists(id))
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        //throw;
        //        return StatusCode(500);
        //    }
        //}
        //return NoContent();

        //Om jag skulle vilja returnera den ändrade posten.
        //var createdTorment = _mapper.Map<TournamentDto>(tournamentExist);
        //return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentExist.Id }, createdTorment);
        //return AcceptedAtAction(nameof(GetTournamentDetails), new { id = tournamentExist.Id }, createdTorment);
        //UpdatedAtAction?

    }

    // POST: api/TournamentDetails
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TournamentDto>> PostTournamentDetails(TournamentCreateDto dto)
    {
        TryValidateModel(dto);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var status = await _serviceManager.TournamentService.CreateAsync(dto);
        if (!status.IsSuccess )
        {
            return StatusCode(status.StatusCode, Wrap(status));
        }
        else
        {
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = status.Id }, status.Data);

        }
        //var torment = _mapper.Map<TournamentDetails>(dto);

        //_context.TournamentDetails.Add(torment);
        //_context.TournamentRepository.Add(torment);
        //_serviceManager.TournamentService.Add(dto);

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

        //var createdTorment = _mapper.Map<TournamentDto>(torment);

        //_context.TournamentDetails.Add(tournamentDetails);
        //await _context.SaveChangesAsync();

        //      return CreatedAtAction("GetTournamentDetails", new { id = tournamentDetails.Id }, tournamentDetails);

    }

    // DELETE: api/TournamentDetails/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTournamentDetails(int id)
    {
        //   var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
        //var tournamentDetails = await _context.TournamentRepository.GetAsync(id);

        //if (tournamentDetails == null)
        //{
        //    return NotFound();
        //}

        //        _context.TournamentDetails.Remove(tournamentDetails);
        // _context.TournamentRepository.Remove(tournamentDetails);

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

        var retur = await _serviceManager.TournamentService.DeleteAsync(id);

        if(retur.IsSuccess)
            return NoContent();
        else
            return StatusCode(retur.StatusCode);
    }

    [HttpPatch("{tournamentId}")]
    public async Task<ActionResult<TournamentDto>> PatchTournament(int tournamentId, JsonPatchDocument<TournamentUpdateDto> patchDocument)
    {
        if (patchDocument is null) return BadRequest("No patchdocument");

//        var tournamentToPatch = await _context.TournamentRepository.GetAsync(tournamentId);
//        if (tournamentToPatch == null) return NotFound("Tournament does not exist");

//        var dto = _mapper.Map<TournamentUpdateDto>(tournamentToPatch);
////        patchDocument.ApplyTo(dto, ModelState); // Här patchas dto:n ihop med patchdokumentet.    //TODO!!!!!!!!!!!!!
//        TryValidateModel(dto);

//        if (!ModelState.IsValid)
//        {
//            //return BadRequest(); //400
//            return UnprocessableEntity(ModelState); //422
//        }

//        _mapper.Map(dto, tournamentToPatch);

//        try
//        {
//            //await _context.SaveChangesAsync();
//            await _context.CompleteAsync();
//        }
//        catch (DBConcurrencyException)
//        {
//            //throw;
//            return StatusCode(500);
//        }

//        var returDto = _mapper.Map<TournamentDto>(tournamentToPatch);

        var retur = await _serviceManager.TournamentService.UpdateAsync(tournamentId, patchDocument);
        if (retur.IsSuccess)
        {
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = retur.Id }, retur.Data);

        }
        else
        {
            return StatusCode(retur.StatusCode,Wrap(retur));
        }
            //return NoContent();
            //return CreatedAtAction(nameof(GetTournamentDetails), new { id = retur.Id }, retur.Data);
        //return AcceptedAtAction(nameof(GetTournamentDetails), new { id = tournamentToPatch.Id }, returDto);
        //ChangedAtAction / UpdatedAt ???


    }




    //private async Task<bool> TournamentDetailsExists(int id)
    //{
    ////    return _context.TournamentDetails.Any(e => e.Id == id);
    //    return await _context.TournamentRepository.AnyAsync(id);
    //}
}
