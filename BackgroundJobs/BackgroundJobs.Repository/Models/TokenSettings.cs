namespace BackgroundJobs.Models;

/// <summary>
/// Token Settings
/// </summary>
public class TokenSettings
{
    /// <summary>
    /// Signing Key
    /// </summary>
    public string SigningKey { get; set; }

    /// <summary>
    /// Issuer
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Audience
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Duration in Millisecond
    /// </summary>
    public double DurationInMillisecond { get; set; }
}

