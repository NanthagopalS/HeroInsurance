using FluentValidation.Results;
using POSP.Core.Contracts.Common;

namespace POSP.Core.Responses;
/// <summary>
/// Base result for Handler
/// </summary>
public class HeroResult : IHeroResult
{
    /// <summary>
    /// Initalize
    /// </summary>
    public HeroResult()
    {

    }

    /// <summary>
    /// Define result is failed
    /// </summary>
    public bool Failed => !Succeeded;

    /// <summary>
    /// Return list of message
    /// </summary>
    public List<string> Messages { get; set; } = new List<string>();

    /// <summary>
    /// Define result is success
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Return list of Validation Failures
    /// </summary>
    public List<ValidationFailure> ValidationFailures { get; set; } = new List<ValidationFailure>();

    /// <summary>
    /// Defined Failed result
    /// </summary>
    /// <returns></returns>
    public static IHeroResult Fail()
    {
        return new HeroResult { Succeeded = false };
    }

    /// <summary>
    /// Defined failed result with  Messages
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static IHeroResult Fail(string message)
    {
        return new HeroResult { Succeeded = false, Messages = new List<string> { message } };
    }

    /// <summary>
    /// Return validation failures
    /// </summary>
    /// <param name="validationFailures"></param>
    /// <returns></returns>
    public static IHeroResult Fail(List<ValidationFailure> validationFailures)
    {
        return new HeroResult { Succeeded = false, ValidationFailures = validationFailures };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static IHeroResult Success()
    {
        return new HeroResult { Succeeded = true };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static IHeroResult Success(string message)
    {
        return new HeroResult { Succeeded = true, Messages = new List<string> { message } };
    }
}

/// <summary>
/// Common Result for Handler
/// </summary>
/// <typeparam name="T"></typeparam>
public class HeroResult<T> : HeroResult, IHeroResult<T>
{
    /// <summary>
    /// Generic Result
    /// </summary>
    public T Result { get; set; }

    /// <summary>
    /// Defined Failed result
    /// </summary>
    /// <returns></returns>
    public static new HeroResult<T> Fail()
    {
        return new HeroResult<T> { Succeeded = false };
    }

    /// <summary>
    /// Defined failed result with  Messages
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static new HeroResult<T> Fail(string message)
    {
        return new HeroResult<T> { Succeeded = false, Messages = new List<string> { message } };
    }

    /// <summary>
    /// Return validation failures
    /// </summary>
    /// <param name="validationFailures"></param>
    /// <returns></returns>
    public static new HeroResult<T> Fail(List<ValidationFailure> validationFailures)
    {
        return new HeroResult<T> { Succeeded = false, ValidationFailures = validationFailures };
    }

    /// <summary>
    /// Fail
    /// </summary>
    /// <param name="Result"></param>
    /// <param name="validationFailures"></param>
    /// <returns></returns>
    public static HeroResult<T> Fail(T Result, List<ValidationFailure> validationFailures)
    {
        return new HeroResult<T> { Succeeded = false, Result = Result, ValidationFailures = validationFailures };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static new HeroResult<T> Success()
    {
        return new HeroResult<T> { Succeeded = true };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static new HeroResult<T> Success(string message)
    {
        return new HeroResult<T> { Succeeded = true, Messages = new List<string> { message } };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static HeroResult<T> Success(T Result)
    {
        return new HeroResult<T> { Succeeded = true, Result = Result };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static HeroResult<T> Success(T Result, string message)
    {
        return new HeroResult<T> { Succeeded = true, Result = Result, Messages = new List<string> { message } };
    }

    /// <summary>
    /// Defined Success result
    /// </summary>
    /// <returns></returns>
    public static HeroResult<T> Success(T Result, List<string> message)
    {
        return new HeroResult<T> { Succeeded = true, Result = Result, Messages = message };
    }
}
