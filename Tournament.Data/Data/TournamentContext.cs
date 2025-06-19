using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tournament.Core.Entities;

namespace Tournament.Data.Data;

public class TournamentContext : DbContext
{
    public TournamentContext (DbContextOptions<TournamentContext> options)
        : base(options)
    {
    }

    //public override EntityEntry Entry(object entity)
    //{

    //}

    public DbSet<Tournament.Core.Entities.TournamentDetails> TournamentDetails { get; set; } = default!;
    public DbSet<Tournament.Core.Entities.Game> Game { get; set; } = default!;
}
