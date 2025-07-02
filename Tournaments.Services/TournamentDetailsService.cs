using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.VisualBasic;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tournaments.Services;

public class TournamentDetailsService(ITournamentUoW _uow, IMapper _mapper) : ITournamentDetailsService
{
    public async Task<ResultObjectDto<IEnumerable<TournamentDto>>> GetAllAsync(bool showGames = false, bool sorted = false)
    {
        ResultObjectDto<IEnumerable<TournamentDto>> retur = new ResultObjectDto<IEnumerable<TournamentDto>>();
        IEnumerable objects = await _uow.TournamentRepository.GetAllAsync(showGames, sorted);
        retur.Data = _mapper.Map<IEnumerable<TournamentDto>>(objects);
        retur.IsSuccess = true;
        return retur;
    }

    public async Task<ResultObjectDto<TournamentDto>> GetAsync(int id, bool showGames = false)
    {
        ResultObjectDto<TournamentDto> retur = new ResultObjectDto<TournamentDto>();
        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(id, showGames);
        retur.Data = _mapper.Map<TournamentDto>(tournament);
        retur.IsSuccess = true;
        return retur;
    }

    public async Task<ResultObjectDto<TournamentDto>> CreateAsync(TournamentCreateDto dto)
    {
        var torment = _mapper.Map<TournamentDetails>(dto);
        _uow.TournamentRepository.Add(torment);

        try
        {
            await _uow.CompleteAsync();
        }
        catch (DBConcurrencyException e)
        {
            return (new ResultObjectDto<TournamentDto>
            {
                IsSuccess = false,
                Message = e.Message,
                StatusCode = 500,
                Data = null,
                Id = torment.Id,
                Pagination = null
            });
        }
        catch (Exception e)
        {
            return (new ResultObjectDto<TournamentDto>()
            {
                IsSuccess = false,
                Message = e.Message,
                StatusCode = 500,
                Data = null,
                Id = torment.Id,
                Pagination = null
            });
        }

        return (new ResultObjectDto<TournamentDto>
        {
            IsSuccess = true,
            Message = string.Empty,
            StatusCode = 200,
            Id = torment.Id,
            Data = _mapper.Map<TournamentDto>(torment),
            Pagination = null
        });


    }

    public async Task<ResultObjectDto<TournamentDto>> UpdateAsync(int Id, JsonPatchDocument<TournamentUpdateDto> patchDocument)
    {
        ResultObjectDto<TournamentDto> retur = new ResultObjectDto<TournamentDto>();
        retur.IsSuccess = false;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 500;

        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(Id, showGames: false);
        if (tournament == null)
        {
            retur.Message = $"Tournament med id={Id} saknas.";
            return retur;
        }
        var dto = _mapper.Map<TournamentUpdateDto>(tournament);

        //TryValidateModel(dto);

        //if (!ModelState.IsValid)
        //{
        //    //return BadRequest(); //400
        //    //return UnprocessableEntity(ModelState); //422
        //}

        _mapper.Map(dto, tournament);
        retur = await _uow.CompleteAsync(retur);
        if (retur.IsSuccess)
        {
            retur.Data = _mapper.Map<TournamentDto>(tournament);
            retur.Message = string.Empty;
            return retur;
        }
        else
        {
            return retur;
        }
        
    }
    public async Task<ResultObjectDto<TournamentDto>> UpdateAsync (TournamentUpdateDto update)
    {
        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(update.Id, showGames:false);
        if (tournament == null)
        {
            return new ResultObjectDto<TournamentDto>
            {
                IsSuccess = false,
                Message = $"Tournament med id={update.Id} saknas.",
                StatusCode = 500,
                Data = null,
                Pagination = null
            };
        }

        var torment = _mapper.Map(update, tournament);

        try
        {
            await _uow.CompleteAsync();
        }
        //      catch (DbUpdateConcurrencyException)
        catch (DBConcurrencyException e)
        {
            return new ResultObjectDto<TournamentDto>
            {
                IsSuccess = false,
                Message = e.Message,
                StatusCode = 500,
                Data = null,
                Pagination = null
            };

            //if (!await TournamentDetailsExists(id))
            //{
            //    return NotFound();
            //}
            //else
            //{
            //    return StatusCode(500);
            //}
        }

        return new ResultObjectDto<TournamentDto>
        {
            IsSuccess = true,
            Message = string.Empty,
            StatusCode = 200,
            Data = _mapper.Map<TournamentDto>(torment),
            Pagination = null
        };
    }

    public async Task<ResultObjectDto<int>> DeleteAsync(int id)
    {
        ResultObjectDto<int> retur = new ResultObjectDto<int>();
        retur.IsSuccess = false;
        retur.Data = -1;
        retur.Pagination = null;
        retur.StatusCode = 500;

        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(id, showGames: false);
        
        if (tournament == null)
        {
            retur.Message = $"Tournament med id={id} saknas.";
            return retur;
        }

        _uow.TournamentRepository.Remove(tournament);
        retur = await _uow.CompleteAsync(retur);
        if (retur.IsSuccess)
        {
            retur.StatusCode = 200;
            retur.Data = id;
        }
        return retur;
    }
}
