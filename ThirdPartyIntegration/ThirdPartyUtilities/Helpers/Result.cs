using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using ThirdPartyUtilities.Models;

namespace ThirdPartyUtilities.Helpers;
/// <summary>
/// Result
/// </summary>
public static class Result
{
    /// <summary>
    /// Success
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static ResultModel CreateSuccess<T>(T data, int statusCode)
    {
        var result = new ResultModel
        {
            Data = data,
            Message = "Success",
            StatusCode = statusCode.ToString(),
        };
        return result;
    }

    /// <summary>
    /// CreateValidationError
    /// </summary>
    /// <param name="validationFailures"></param>
    /// <returns></returns>
    public static ResultModel CreateValidationError(List<ValidationFailure> validationFailures)
    {
        ModelStateDictionary keyValuePairs = new ModelStateDictionary();
        foreach (var validationFailure in validationFailures)
        {
            keyValuePairs.AddModelError(validationFailure.PropertyName, validationFailure.ErrorMessage);
        }

        var problemDetails = new ValidationProblemDetails(keyValuePairs)
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = HttpStatusCode.BadRequest.ToString(),
            Detail = "One or more validation errors occured"
        };

        var result = new ResultModel
        {
            Data = problemDetails,
            Message = "Failed",
            StatusCode = ((int)HttpStatusCode.BadRequest).ToString()
        };
        return result;
    }

    /// <summary>
    /// CreateValidationError
    /// </summary>
    /// <param name="lstError"></param>
    /// <returns></returns>
    public static ResultModel CreateValidationError(List<string> lstError)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = HttpStatusCode.BadRequest.ToString(),
            Detail = string.Join(",", lstError.Select(x => $"{x} "))
        };

        var result = new ResultModel
        {
            Data = problemDetails,
            Message = "Failed",
            StatusCode = ((int)HttpStatusCode.BadRequest).ToString()
        };
        return result;
    }

    /// <summary>
    /// CreateNotFoundError
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static ResultModel CreateNotFoundError(string error)
    {
        //Read err.json file
        //create model wrt error.json
        //desrialize
        //
        //filter the errorcode from collection --FirstOrDefault


        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = HttpStatusCode.NotFound.ToString(),
            Detail = error
        };

        var result = new ResultModel
        {
            Data = problemDetails,
            Message = "Failed",
            StatusCode = ((int)HttpStatusCode.NotFound).ToString()
        };
        return result;
    }

    /// <summary>
    /// CreateNotFoundError
    /// </summary>
    /// <param name="lstError"></param>
    /// <returns></returns>
    public static ResultModel CreateNotFoundError(List<string> lstError)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = HttpStatusCode.NotFound.ToString(),
            Detail = string.Join(",", lstError.Select(x => $"{x} "))
        };

        var result = new ResultModel
        {
            Data = problemDetails,
            Message = "Failed",
            StatusCode = ((int)HttpStatusCode.NotFound).ToString()
        };
        return result;
    }

    /// <summary>
    /// CreateUnAuthorizedError
    /// </summary>
    /// <param name="lstError"></param>
    /// <returns></returns>
    public static ResultModel CreateUnAuthorizedError(string error)
    {
        var result = new ResultModel
        {
            Data = error,
            Message = HttpStatusCode.Unauthorized.ToString(),
            StatusCode = ((int)HttpStatusCode.Unauthorized).ToString()
        };
        return result;
    }

    public static ResultModel CreatePdfSuccess<T>(T html, int statusCode)
    {
        var result = new ResultModel
        {
            Data = html,
            Message = "Success",
            StatusCode = statusCode.ToString(),
        };
        return result;
    }

    public static ResultModel CreateOtpUnSuccess<T>(T data, int statusCode)
    {
        var result = new ResultModel
        {
            Data = data,
            Message = "Wrong OTP",
            StatusCode = statusCode.ToString(),
        };
        return result;
    }
}
