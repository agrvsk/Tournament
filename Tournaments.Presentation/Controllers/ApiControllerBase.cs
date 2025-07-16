using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tournament.Core.Responses;
using Tournament.Shared.Requests;

namespace Tournaments.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiControllerBase : ControllerBase
{
    [NonAction]
    public void Add2Header(MetaData paginering)
    {
        if (paginering == null) return;
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginering));
    }

    [NonAction]
    public ActionResult ProcessError(ApiBaseResponse baseResponse)
    {
        return baseResponse switch
        {
            // Kräver Microsoft.AspNetCore.Mvc.NewtonsoftJson
            ApiNotFoundResponse => NotFound
            (
                Results.Problem 
                (
                detail: ((ApiNotFoundResponse)baseResponse).Message,
                statusCode: StatusCodes.Status404NotFound,
                title: "Not found",
                instance: HttpContext.Request.Path
                )
            ),
            _ => throw new NotImplementedException()
        }; 
    }
}
