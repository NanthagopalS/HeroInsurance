using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using ThirdPartyUtilities.Models;

namespace ThirdPartyUtilities.Configuration;

public class SignzyLogsDBContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly SignzyDBConnectModel _dbConnection;
    /// <summary>
    /// Initialize  
    /// </summary>
    /// <param name="configuration"></param>
    public SignzyLogsDBContext(IConfiguration configuration, IOptions<SignzyDBConnectModel> options)
    {
        _configuration = configuration;
        _connectionString = options.Value.LogsSqlConnection;
    }
    /// <summary>
    /// CreateConnection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}