using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Entities;
using Tournament.Data.Data;
using Tournaments.Presentation.Controllers;

namespace Tournament.Api.Tests.TestFixtures;

public class RepoControllerFixture : IDisposable
{
    public Mock<IServiceManager> ServiceManagerMock { get; }
    public Mapper Mapper { get; }
    public Mock<UserManager<User>> UserManager { get; }
    public AuthController Sut { get; }

    public RepoControllerFixture()
    {
        ServiceManagerMock = new Mock<IServiceManager>();

        Mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TournamentMappings>();
        }));

        var mockUserStore = new Mock<IUserStore<User>>();
        UserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

//          Sut = new AuthController(ServiceManagerMock.Object, Mapper, UserManager.Object);
        Sut = new AuthController(ServiceManagerMock.Object);
    }

    public List<User> GetUsers()
    {
        return new List<User>
        {
            new User
            {
                 Id = "1",
//                     Name = "Kalle",
//                     Age = 12,
                 UserName = "Kalle"
            },
           new User
            {
                 Id = "2",
//                     Name = "Kalle",
//                     Age = 12,
                 UserName = "Kalle"
            },
        };

    }


    public void Dispose()
    {
        // Not used here
    }
}
