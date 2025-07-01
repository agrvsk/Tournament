using System.Collections;
using AutoMapper;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
namespace Tournaments.Services;

public class GameService(ITournamentUoW _uow, IMapper _mapper) : IGameService
{
    public async Task<(IEnumerable<GameDto>,PaginationMetadata)> GetAllAsync(bool sorted = false, int pageNr = 1, int pageSize = 10)
    {
        (IEnumerable objects, PaginationMetadata pg) = await _uow.GameRepository.GetAllAsync(sorted, pageNr, pageSize);
        IEnumerable<GameDto> dtos = _mapper.Map<IEnumerable<GameDto>>(objects);
        return (dtos, pg);
    }

    public async Task<GameDto> GetAsync(int id)
    {
        Game? game = await _uow.GameRepository.GetAsync(id);
        return _mapper.Map<GameDto>(game);
    }

    public async Task<IEnumerable<GameDto>> GetByTitleAsync(string title)
    {
        IEnumerable games = await _uow.GameRepository.GetByTitleAsync(title);
        return _mapper.Map<IEnumerable<GameDto>>(games);
    }
}
