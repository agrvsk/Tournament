using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Exceptions;

public abstract class NotAllowedException(string Message, string Title = "Not allowed") : Exception(Message) 
{
    public string Title { get; set; }
}
public sealed class TournamentFullException(int Id) : NotAllowedException($"No more games can be added to the tournament with id {Id}.")
{
}

