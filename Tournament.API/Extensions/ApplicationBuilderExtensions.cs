using System;
using Bogus;
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
                var companies = GenerateTournaments(4);
                db.AddRange(companies);
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
            c.Games = GenerateGames(f.Random.Int(min: 2, max: 10) );
        });

        return faker.Generate(nrOfTournaments);

    }

    private static ICollection<Game> GenerateGames(int nrOfGames)
    {
        //string[] positions = { "Developer", "Tester", "Manager" };
        var faker = new Faker<Game>("sv").Rules((f, e) =>
        {
            e.Title = $"{f.Address.City()} Cup";

            e.Time = f.Date.BetweenDateOnly(
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddYears(10))
            ).ToDateTime(f.Date.BetweenTimeOnly(
                TimeOnly.Parse("00:00 AM"),
                TimeOnly.Parse("11:59 PM")
                ));


            //e.Time = DateOnly.FromDateTime(dag)
            //.ToDateTime(f.Date.BetweenTimeOnly(
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
