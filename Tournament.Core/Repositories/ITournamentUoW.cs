using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;

namespace Tournament.Core.Repositories;

public interface ITournamentUoW
{

    ITournamentRepository TournamentRepository { get; }
    IGameRepository GameRepository { get; }
    Task CompleteAsync();
    Task<ResultObjectDto<T>> CompleteAsync<T>(ResultObjectDto<T> input);
}
