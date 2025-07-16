using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Tournament.Shared.DTOs;
using Tournament.Core.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Tournament.Data.Data;

public class TournamentMappings : Profile
{
    public TournamentMappings()
    {
        CreateMap<TournamentDetails, TournamentDto>();
        CreateMap<TournamentDetails, TournamentCreateDto>().ReverseMap();
        CreateMap<TournamentDetails, TournamentUpdateDto>().ReverseMap();

        CreateMap<Game, GameDto>();
        CreateMap<Game, GameCreateDto>().ReverseMap();
        CreateMap<Game, GameUpdateDto>().ReverseMap();

    //    CreateMap<User, EmployeeUpdateDto>().ReverseMap();
        CreateMap<UserForRegistrationDto, User>().ReverseMap();

    }
}
