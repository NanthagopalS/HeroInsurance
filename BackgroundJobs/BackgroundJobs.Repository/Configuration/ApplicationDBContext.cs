using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace BackgroundJobs.Configuration;

/// <summary>
/// Application DB Context
/// </summary>
public class ApplicationDBContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    /// <summary>
    /// Initialize  
    /// </summary>
    /// <param name="configuration"></param>
    public ApplicationDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("SqlConnection");
    }

    /// <summary>   
    /// CreateConnection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}

