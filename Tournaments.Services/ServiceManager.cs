using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contracts;
using Services.Contracts;

namespace Tournaments.Services;

public class ServiceManager(
    Lazy<ITournamentDetailsService> tournamentservice, 
    Lazy<IGameService> gameservice,
    Lazy<IAuthService> authService
    ) : IServiceManager
{
    public ITournamentDetailsService TournamentService => tournamentservice.Value;
    public IGameService GameService => gameservice.Value;
    public IAuthService AuthService => authService.Value;
}
