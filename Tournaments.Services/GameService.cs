using System.Collections;
using AutoMapper;
using Service.Contracts;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
namespace Tournaments.Services;

public class GameService(ITournamentUoW _uow, IMapper _mapper) : IGameService
{
  //public async Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(bool sorted = false, int pageNr = 1, int pageSize = 20)
    public async Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(GameRequestParams gParams)
    {
        ResultObjectDto<IEnumerable<GameDto>> retur = new ResultObjectDto<IEnumerable<GameDto>>();
        retur.Message = string.Empty;
        retur.IsSuccess = false;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 500;

        (IEnumerable objects, PaginationMetadataDto pg) = await _uow.GameRepository.GetAllAsync(gParams);
        retur.Pagination = pg;

        if (objects == null)
        {
            retur.Message = $"No Game was found";
            return retur;
        }

        IEnumerable<GameDto> dtos = _mapper.Map<IEnumerable<GameDto>>(objects);
        retur.IsSuccess = true;
        retur.StatusCode=200;
        retur.Data = dtos;
        //return (dtos, pg);
        return (retur);
    }

    public async Task<ResultObjectDto<GameDto>> GetAsync(int id)
    {
        ResultObjectDto<GameDto> retur = new ResultObjectDto<GameDto>();
        retur.Message = string.Empty;
        retur.IsSuccess = false;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 500;

        Game? game = await _uow.GameRepository.GetAsync(id);
        if (game == null)
        {
            retur.Message = $"Game with id {id} was not found.";
            return retur;
        }
        retur.IsSuccess = true;
        retur.StatusCode = 200;
        retur.Data = _mapper.Map<GameDto>(game);
        return (retur);
    }

    //public async Task<ResultObjectDto<IEnumerable<GameDto>>> GetByTitleAsync(string title, int pageNr = 1, int pageSize = 20)
    //{
    //    ResultObjectDto<IEnumerable<GameDto>> retur = new ResultObjectDto<IEnumerable<GameDto>>();
    //    retur.Message = string.Empty;
    //    retur.IsSuccess = false;
    //    retur.Data = null;
    //    retur.Pagination = null;
    //    retur.StatusCode = 500;

    //    (IEnumerable games, var pg) = await _uow.GameRepository.GetByTitleAsync(title, pageNr ,  pageSize );
    //    if (games == null)
    //    {
    //        retur.Message = $"No game with title {title} was not found.";
    //        return retur;
    //    }
    //    retur.IsSuccess = true;
    //    retur.Data = _mapper.Map<IEnumerable<GameDto>>(games);
    //    retur.Pagination = pg;
    //    retur.StatusCode = 200;
    //    return retur;
    //}


    public async Task<ResultObjectDto<GameDto>> CreateAsync(GameCreateDto dto)
    {
        ResultObjectDto<GameDto> retur = new ResultObjectDto<GameDto>();
        retur.IsSuccess = false;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 500;

        var game = _mapper.Map<Game>(dto);
        _uow.GameRepository.Add(game);

        retur = await _uow.CompleteAsync(retur);
        if (retur.IsSuccess)
        {
            retur.Message = string.Empty;
            retur.StatusCode = 201;
            retur.Id = game.Id;
            retur.Data = _mapper.Map<GameDto>(game);
            retur.Pagination = null;
        }
        return retur;
    }

    public async Task<ResultObjectDto<GameDto>> UpdateAsync(GameUpdateDto update)
    {
        ResultObjectDto<GameDto> retur = new ResultObjectDto<GameDto>();
        retur.IsSuccess = false;
        retur.Data = null;
        retur.Pagination = null;
        retur.StatusCode = 500;

        Game? game = await _uow.GameRepository.GetAsync(update.Id);
        if (game == null)
        {
            retur.Message = $"Tournament med id={update.Id} saknas.";
            return retur;
        }

        var torment = _mapper.Map(update, game);
        retur = await _uow.CompleteAsync(retur);
        if (retur.IsSuccess)
        {
            retur.StatusCode = 204;
            retur.Id = update.Id;
            retur.Message = string.Empty;
            retur.Data = _mapper.Map<GameDto>(torment);
        }
        return retur;



    }


    public async Task<ResultObjectDto<int>> DeleteAsync(int id)
    {
        ResultObjectDto<int> retur = new ResultObjectDto<int>();
        retur.IsSuccess = false;
        retur.Data = -1;
        retur.Pagination = null;
        retur.StatusCode = 500;

        Game? game = await _uow.GameRepository.GetAsync(id);

        if (game == null)
        {
            retur.Message = $"Game med id={id} saknas.";
            return retur;
        }

        _uow.GameRepository.Remove(game);
        retur = await _uow.CompleteAsync(retur);
        if (retur.IsSuccess)
        {
            retur.StatusCode = 204;
            retur.Data = id;
        }
        return retur;
    }

}
