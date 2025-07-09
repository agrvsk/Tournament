using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;
using Tournament.Core.Requests;

namespace Tournament.Core.Repositories;

public interface IGameRepository
{
  //Task<(IEnumerable<Game>, PaginationMetadataDto)> GetAllAsync(bool sort = false, int pageNr = 1, int pageSize = 20);
    Task<(IEnumerable<Game>, PaginationMetadataDto)> GetAllAsync(GameRequestParams gParams);
    Task<Game> GetAsync(int id);
  //Task<(IEnumerable<Game>, PaginationMetadataDto)> GetByTitleAsync(string title, int pageNr = 1, int pageSize = 20);
    Task<bool> AnyAsync(int id);
    void Add(Game game);
    void Update(Game game);
    void Remove(Game game);

    void SetStateModified(Game game);
}
