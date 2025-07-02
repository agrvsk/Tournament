using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournaments.Services;

public class TournamentDetailsService(ITournamentUoW _uow, IMapper _mapper) : ITournamentDetailsService
{
    public async Task<IEnumerable<TournamentDto>> GetAllAsync(bool showGames = false, bool sorted = false)
    {
        IEnumerable objects = await _uow.TournamentRepository.GetAllAsync(showGames, sorted);
        return _mapper.Map<IEnumerable<TournamentDto>>(objects);
    }

    public async Task<TournamentDto> GetAsync(int id, bool showGames = false)
    {
        TournamentDetails? tournament = await _uow.TournamentRepository.GetAsync(id, showGames);
        return _mapper.Map<TournamentDto>(tournament);
    }

    public async Task<(TournamentDto, ResultObject)> CreateAsync(TournamentCreateDto dto)
    {
        var torment = _mapper.Map<TournamentDetails>(dto);
        _uow.TournamentRepository.Add(torment);

        try
        {
            await _uow.CompleteAsync();
        }
        catch (DBConcurrencyException e)
        {
            return (null,  new ResultObject
            (
                IsSuccess: false,
                Message: e.Message,
                StatusCode: 500,
                Id: torment.Id,
                pagination: null
            ));
            //throw e;
            //return StatusCode(500);
        }
        catch (Exception e)
        {
            return (null, new ResultObject
            (
                IsSuccess: false,
                Message: e.Message,
                StatusCode: 500,
                Id: torment.Id,
                pagination: null
            ));
        }

        ResultObject r= new 
        (
            IsSuccess: true,
            Message: string.Empty,
            StatusCode: 200,
            Id: torment.Id,
            pagination: null
        );

        var data = _mapper.Map<TournamentDto>(torment);
        return (data , r);
    }


    public Task UpdateAsync(TournamentUpdateDto update)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
