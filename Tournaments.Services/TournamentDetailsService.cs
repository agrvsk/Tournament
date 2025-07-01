using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}
