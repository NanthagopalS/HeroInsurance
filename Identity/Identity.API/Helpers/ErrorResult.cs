using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Identity.API.Helpers;

/// <summary>
/// Error Result
/// </summary>
public static class ErrorResult
{
    /// <summary>
    /// CreateValidationError for List of string
    /// </summary>
    /// <param name="lstError"></param>`
    /// <returns></returns>
    public static ProblemDetails CreateValidationError(List<string> lstError)
    {
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = HttpStatusCode.BadRequest.ToString(),
            Detail = String.Join(",", lstError.Select(x => $"{x}")),
        };
    }

    /// <summary>
    /// CreateNotFoundError
    /// </summary>
    /// <param name="lstError"></param>
    /// <returns></returns>
    public static ProblemDetails CreateNotFoundError(List<string> lstError)
    {
        return new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = HttpStatusCode.NotFound.ToString(),
            Detail = String.Join(",", lstError.Select(x => $"{x} ")),
        };
    }
}
