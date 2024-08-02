using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BackgroundJobs.Helpers;


/// <summary>
/// HttpGlobalExceptionHandler
/// </summary>
public static class HttpGlobalExceptionHandler
{
    /// <summary>
    /// ExceptionHandler
    /// </summary>
    /// <param name="context"></param>
    /// <param name="app"></param>
    /// <returns></returns>
    public static async Task ExceptionHandler(HttpContext context, WebApplication app)
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        context.Response.ContentType = "application/problem+json";
        var problemDetails = new ProblemDetails();

        var json = new List<string>();

        if (app.Environment.IsDevelopment())
        {
            json.Add("An error occur.Try it again");
        }
        else
        {
            json.Add(exception.Message);
        }

        var errResponse = json;

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errResponse));

    }
}
