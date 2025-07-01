using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Service.Contracts;

public interface IGameService
{
    Task<(IEnumerable<GameDto>, PaginationMetadata)> GetAllAsync(bool sorted=false, int pageNr=1, int pageSize=10);
    Task<IEnumerable<GameDto>> GetByTitleAsync(string title);
    Task<GameDto> GetAsync(int id);
}