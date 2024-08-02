using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Insurance.Persistence.Configuration;

public class LogsDBContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    /// <summary>
    /// Initialize  
    /// </summary>
    /// <param name="configuration"></param>
    public LogsDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("LogsSqlConnection");
    }

    /// <summary>
    /// CreateConnection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
