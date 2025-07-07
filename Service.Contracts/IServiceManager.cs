using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts;

namespace Service.Contracts;

public interface IServiceManager
{
    ITournamentDetailsService TournamentService { get; }
    IGameService GameService { get; }
    IAuthService AuthService { get; }
}
