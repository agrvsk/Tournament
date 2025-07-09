using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tournament.Core.Exceptions;

namespace Tournament.Api.Extensions;

public static class ExceptionMiddleware
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var contextFeatures = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeatures != null)
                {
                    var problemDetailsFactory = app.Services.GetService<ProblemDetailsFactory>();
                    ArgumentNullException.ThrowIfNull(nameof(problemDetailsFactory));

                    var problemDetails = CreateProblemDetails(context, contextFeatures.Error, problemDetailsFactory, app);

                    context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                }
            });
        });
    }

    private static ProblemDetails CreateProblemDetails(HttpContext context, Exception error, ProblemDetailsFactory? problemDetailsFactory, WebApplication app)
    {
        return error switch
        {
            TournamentNotFoundException tournamentNotFoundException
            => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound,
                title: tournamentNotFoundException.Title,
                type: context.Request.Method,
                detail: tournamentNotFoundException.Message,
                instance: context.Request.Path),

            GameNotFoundException gameNotFoundException
            => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status404NotFound,
                title: gameNotFoundException.Title,
                type: context.Request.Method,
                detail: gameNotFoundException.Message,
                instance: context.Request.Path),

            TournamentFullException tournamentFullException
            => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status405MethodNotAllowed,
                title: tournamentFullException.Title,
                type: context.Request.Method,
                detail: tournamentFullException.Message,
                instance: context.Request.Path),

            _ => problemDetailsFactory.CreateProblemDetails(
                context,
                StatusCodes.Status500InternalServerError,
                title: "Internal Server Error",
                type: context.Request.Method,
                detail: app.Environment.IsDevelopment() ? error.Message : "Un unexpected error occured.")
        };
    }
}
