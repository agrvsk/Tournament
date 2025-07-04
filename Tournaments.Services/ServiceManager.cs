﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contracts;

namespace Tournaments.Services;

public class ServiceManager(
    Lazy<ITournamentDetailsService> tournamentservice, 
    Lazy<IGameService> gameservice
    ) : IServiceManager
{
    public ITournamentDetailsService TournamentService => tournamentservice.Value;
    public IGameService GameService => gameservice.Value;
}
