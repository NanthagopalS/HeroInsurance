using Microsoft.AspNetCore.Mvc;

namespace POSP.API.Helpers;

/// <summary>
/// InternalServerErrorObjectResult
/// </summary>
public class InternalServerErrorObjectResult :ObjectResult
{
    /// <summary>
    /// Initialize InternalServerErrorObjectResult
    /// </summary>
    /// <param name="error"></param>
    public InternalServerErrorObjectResult(object error) :base(error)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}

/// <summary>
/// JsonErrorResponse
/// </summary>
public class JsonErrorResponse
{
    /// <summary>
    /// Messgages
    /// </summary>
    public object Messgages { get; set; }
}
