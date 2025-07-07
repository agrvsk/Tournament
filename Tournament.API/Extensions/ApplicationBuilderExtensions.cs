using System;
using System.Configuration;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Data.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    private static UserManager<User> userManager;
    private static RoleManager<IdentityRole> roleManager;
    private static IConfiguration configuration;

    private const string gameRole = "Game";
    private const string adminRole = "Admin";


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
                userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                configuration = serviceProvider.GetRequiredService<IConfiguration>();

                await CreateRolesAsync(new[] { adminRole, gameRole });

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

    private static async Task CreateRolesAsync(string[] roleNames)
    {
        foreach (var roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }
            var role = new IdentityRole { Name = roleName };
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors));
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
