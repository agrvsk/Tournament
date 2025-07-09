using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Requests;
using Tournament.Core.Responses;

namespace Service.Contracts;

public interface IGameService
{
    //Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(bool sorted = false, int pageNr = 1, int pageSize = 20);
    Task<ApiBaseResponse> GetAllAsync(GameRequestParams gParams);
//  Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(GameRequestParams gParams);
  //Task<ResultObjectDto<IEnumerable<GameDto>>> GetByTitleAsync(string title, int pageNr = 1, int pageSize = 20);
    Task<ResultObjectDto<GameDto>> GetAsync(int id);

    Task<ResultObjectDto<GameDto>> CreateAsync(GameCreateDto create);
    Task<ResultObjectDto<GameDto>> UpdateAsync(GameUpdateDto update);
//  Task<ResultObjectDto<GameDto>> UpdateAsync(int Id, JsonPatchDocument<GameUpdateDto> patchDocument);
    Task<ResultObjectDto<int>> DeleteAsync(int id);

}