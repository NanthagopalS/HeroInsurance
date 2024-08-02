namespace Identity.Core.Exceptions;

/// <summary>
/// NotFound Exception
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// NotFound Exception
    /// </summary>
    public NotFoundException()
        : base()
    {
    }

    /// <summary>
    /// NotFound Exception
    /// </summary>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// NotFound Exception
    /// </summary>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// NotFound Exception
    /// </summary>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}
