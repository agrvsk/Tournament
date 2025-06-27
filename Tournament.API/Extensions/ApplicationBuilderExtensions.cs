using System;
using Bogus;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task SeedDataAsync(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var db = serviceProvider.GetRequiredService<TournamentContext>();

            await db.Database.MigrateAsync();
            if (await db.TournamentDetails.AnyAsync())
            {
                return; // Database has been seeded
            }

            try
            {
                var torments = GenerateTournaments(4);
                db.AddRange(torments);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }



    private static List<TournamentDetails> GenerateTournaments(int nrOfTournaments)
    {
        var faker = new Faker<TournamentDetails>("sv").Rules((f, c) =>
        {
            c.Title = $"{f.Address.City()} Open";

            c.StartDate = f.Date.BetweenDateOnly(
                DateOnly.FromDateTime(DateTime.Now),
                DateOnly.FromDateTime(DateTime.Now.AddYears(10))
                ).ToDateTime(TimeOnly.Parse("00:00 PM"));

            c.Games = GenerateGames(f.Random.Int(min: 2, max: 10), c.StartDate );
        });

        return faker.Generate(nrOfTournaments);

    }

    static DateTime nextGame = DateTime.Now;

    private static ICollection<Game> GenerateGames(int nrOfGames, DateTime dt )
    {
        nextGame = dt;

        //DateTime slutdt = dt.AddMonths(3);
        //DateTime[] tPeriod = { dag };
        //string[] positions = { "Developer", "Tester", "Manager" };

        var faker = new Faker<Game>("sv").Rules((f, e) =>
        {
            e.Title = $"{f.Address.City()} Cup";



            //e.Time = DateOnly.FromDateTime(dt)
            //.ToDateTime(f.Date.BetweenTimeOnly(
            //    TimeOnly.Parse("00:00 AM"),
            //    TimeOnly.Parse("11:59 PM")
            // ));
            e.Time = nextGame;
            nextGame = nextGame.AddDays(1);

            //e.Time = f.Date.BetweenDateOnly(
            //DateOnly.FromDateTime(dt),
            //DateOnly.FromDateTime(slutdt)
            //).ToDateTime(f.Date.BetweenTimeOnly(
            //    TimeOnly.Parse("00:00 AM"),
            //    TimeOnly.Parse("11:59 PM")
            //    ));





            //e.Name = f.Person.FullName;
            //e.Age = f.Random.Int(min: 18, max: 70);
            //e.Position = f.PickRandom(positions);
        });

        return faker.Generate(nrOfGames);
    }

}
