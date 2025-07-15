using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Data.Data;
using Tournaments.Presentation.Controllers;

namespace Tournament.Api.Tests.TestFixtures;

public class DatabasFixture : IDisposable
{   
    public TournamentContext Context { get; }
    public AuthController Sut { get; }
    public DatabasFixture()
    {
        var mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TournamentMappings>();
        }));

        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
        var options = new DbContextOptionsBuilder<TournamentContext>()
            .UseSqlServer(configuration.GetConnectionString("TestConnection")).Options;

        Context = new TournamentContext(options);

//            Sut = new AuthController(Context, mapper);

        Context.Database.Migrate();

//          SeedData();
        Context.SaveChanges();
    }

    //private void SeedData()
    //{
    //    Context.Companies.AddRange([
    //                           new Company()
    //                                    {
    //                                      Name = "TestCompanyName",
    //                                      Address = "TestAdress",
    //                                      Country = "TestCountry",
    //                                      Employees =
    //                                          [
    //                                            new ApplicationUser
    //                                            {
    //                                                UserName = "TestUserName",
    //                                                Email = "test@test.com",
    //                                                Age = 50,
    //                                                Name = "TestName",
    //                                                Position = "TestPosition"
    //                                            }
    //                                          ]
    //                                    }
    //                           ]);
    //}

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
