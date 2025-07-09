using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Requests;

namespace Tournament.Core.Responses;

public abstract class ApiBaseResponse(bool Success, MetaData? Paginering=null)
{
    public bool Success { get; set; } = Success;
    public MetaData? Paginering { get; set; } = Paginering;

    //protected ApiBaseResponse(bool success)
    //{
    //    Success = success;
    //}

    public TResultType GetOkResult<TResultType>()
    {
        if(this is ApiOkResponse<TResultType> apiOkResponse)
        {
            return apiOkResponse.Result;
        }
        throw new InvalidOperationException($"Response type {GetType().Name} is not ApiOkResponse");
    }

}

public sealed class ApiOkResponse<TResult>(TResult Result, MetaData? Paginering = null) : ApiBaseResponse(true, Paginering)
{
    public TResult Result { get; set; } = Result;
    //public MetaData? Paginering { get; set; }
    //public ApiOkResponse(TResult result) : base(true)
    //{
    //    Result = result;
    //}
}

public abstract class ApiNotFoundResponse(string Message) : ApiBaseResponse(false)
{
    public string Message { get;}

    //protected ApiNotFoundResponse(string message) : base(false)
    //{
    //    Message = message;
    //}
}

public class TournamentNotFoundResponse(int Id) : ApiNotFoundResponse($"Tournament with id {Id} not found.")
{
    //public CompanyNotFoundResponse(int id) : base($"Company with id {id} not found.")
    //{
        
    //}
}
public class GameNotFoundResponse(int Id) : ApiNotFoundResponse($"Game with id {Id} not found.") 
{ }

