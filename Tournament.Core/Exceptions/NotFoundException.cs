using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Exceptions;

public abstract class NotFoundException(string Message, string Title = "Not found") : Exception(Message)
{
    public string Title { get; set; }
    //protected NotFoundException(string message, string title = "Not found") : base(message) 
    //{
    //    Title = title;
    //}
}

public sealed class TournamentNotFoundException(int Id) : NotFoundException($"The tournament with id {Id} is not found")
{
    //public CompanyNotFoundException(int id) : base($"The tournament with id {id} is not found")
    //{
    //}
}

public sealed class GameNotFoundException(int Id) : NotFoundException($"The game with id {Id} is not found")
{
    //public GameNotFoundException(int id) : base($"The game with id {id} is not found")
    //{
    //}
}
