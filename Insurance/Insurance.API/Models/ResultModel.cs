namespace Insurance.API.Models;

/// <summary>
/// Resut wrapper
/// </summary>
public class ResultModel
{
    /// <summary>
    /// Status Code
    /// </summary>
    public string StatusCode { get; set; }
    
    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// Data
    /// </summary>
    public dynamic Data { get; set; }
}
