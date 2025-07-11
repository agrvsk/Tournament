using System.Collections;
using System.Diagnostics.Metrics;
using AutoMapper;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Exceptions;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using Tournament.Core.Responses;
namespace Tournaments.Services;

public class GameService(ITournamentUoW _uow, IMapper _mapper) : IGameService
{
    //public async Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(bool sorted = false, int pageNr = 1, int pageSize = 20)
    //public async Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(GameRequestParams gParams)
    public async Task<ApiBaseResponse> GetAllAsync(GameRequestParams gParams)
    {
        //(IEnumerable objects, PaginationMetadataDto pg) 
        var pgList = await _uow.GameRepository.GetAllAsync(gParams);

        IEnumerable<GameDto> dtos = _mapper.Map<IEnumerable<GameDto>>(pgList.Items);
        ApiOkResponse<IEnumerable<GameDto>> res = new ApiOkResponse<IEnumerable<GameDto>>(dtos, pgList.MetaData);
        return res;
    }

    public async Task<ApiBaseResponse> GetAsync(int id)
    {
        Game? game = await _uow.GameRepository.GetAsync(id);
        if (game == null)
        {
            //ApiNotFoundResponse res =
            return new GameNotFoundResponse(id);
        }

        GameDto dto = _mapper.Map<GameDto>(game);
        return new ApiOkResponse<GameDto>(dto); //, pgList.MetaData
    }

    public async Task<ApiBaseResponse> CreateAsync(GameCreateDto dto)
    {
        //Max 10 Games/Tournament
        if ( await _uow.GameRepository.GetGameCount(dto.TournamentDetailsId) > 9)
        {
            throw new TournamentFullException(dto.TournamentDetailsId);
        }

        var game = _mapper.Map<Game>(dto);
        _uow.GameRepository.Add(game);
        await _uow.CompleteAsync();

        var retur = _mapper.Map<GameUpdateDto>(game);   //Dto som innehåller ID
        return new ApiOkResponse<GameUpdateDto>(retur); //, pgList.MetaData
    }

    public async Task<ApiBaseResponse> UpdateAsync(GameUpdateDto update)
    {
        Game? game = await _uow.GameRepository.GetAsync(update.Id);
        if (game == null)
        {
            throw new GameNotFoundException(update.Id);
        }

        var torment = _mapper.Map(update, game);
        await _uow.CompleteAsync();

        var dto = _mapper.Map<GameUpdateDto>(torment);
        return new ApiOkResponse<GameUpdateDto>(dto); //, pgList.MetaData
    }


    public async Task<ApiBaseResponse> DeleteAsync(int id)
    {
        Game? game = await _uow.GameRepository.GetAsync(id);
        if (game == null)
        {
            throw new GameNotFoundException(id);
        }

        _uow.GameRepository.Remove(game);
        await _uow.CompleteAsync();

        return new ApiOkResponse<int>(id); //, pgList.MetaData

    }

}
