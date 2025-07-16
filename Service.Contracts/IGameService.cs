using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Tournament.Shared.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Responses;
using Tournament.Shared.Requests;

namespace Service.Contracts;

public interface IGameService
{
    //Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(bool sorted = false, int pageNr = 1, int pageSize = 20);
    Task<ApiBaseResponse> GetAllAsync(GameRequestParams gParams);
//  Task<ResultObjectDto<IEnumerable<GameDto>>> GetAllAsync(GameRequestParams gParams);
  //Task<ResultObjectDto<IEnumerable<GameDto>>> GetByTitleAsync(string title, int pageNr = 1, int pageSize = 20);
    Task<ApiBaseResponse> GetAsync(int id);

    Task<ApiBaseResponse> CreateAsync(GameCreateDto create);
    Task<ApiBaseResponse> UpdateAsync(GameUpdateDto update);
//  Task<ResultObjectDto<GameDto>> UpdateAsync(int Id, JsonPatchDocument<GameUpdateDto> patchDocument);
    Task<ApiBaseResponse> DeleteAsync(int id);

}