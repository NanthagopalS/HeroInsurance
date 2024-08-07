﻿using FluentValidation;
using Identity.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using ThirdPartyUtilities.Helpers;

namespace Identity.API.Helpers;

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
        switch (exception)
        {
            case ValidationException e:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = HttpStatusCode.BadRequest.ToString();
                problemDetails.Detail = String.Join(",", e.Errors.Select(x => $"{x.ErrorMessage} "));
                //e.erros me error code milega ....resluk ke andar naya method ma send karen aa....yaha join karna hai 
                //Controller and validator se karna hai
                var problemResponse = Result.CreateValidationError(e.Errors.ToList());
                //var responsedetail = Result.CreateNotFoundError(e.Errors.FirstOrDefault().ErrorCode);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(JsonSerializer.Serialize(problemResponse));
                //await context.Response.WriteAsync(JsonSerializer.Serialize(responsedetail));
                break;            
            case NotFoundException e:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = HttpStatusCode.NotFound.ToString();
                problemDetails.Detail = exception.Message;
                var notFoundResponse = Result.CreateNotFoundError(exception.Message);

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync(JsonSerializer.Serialize(notFoundResponse));
                break;
            default:
                var json = new List<string>();

                if (app.Environment.IsDevelopment())
                {
                    json.Add("An error occur.Try it again");
                }
                else
                {
                    json.Add(exception.Message);
                }

                var errResponse = Result.CreateValidationError(json);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(errResponse));
                break;
        }
    }
}
