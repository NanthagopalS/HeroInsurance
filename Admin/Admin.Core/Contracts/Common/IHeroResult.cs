namespace Admin.Core.Contracts.Common;
/// <summary>
/// Interface for common result
/// </summary>
public interface IHeroResult
{
    /// <summary>
    /// Messages
    /// </summary>
    List<string> Messages { get; set; }

    /// <summary>
    /// Success
    /// </summary>
    bool Succeeded { get; set; }
}

/// <summary>
/// IHeroResult
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHeroResult<out T> : IHeroResult
{
    /// <summary>
    /// T Result
    /// </summary>
    T Result { get; }
}

