using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BackgroundJobs.Configuration;

public class IdentityApplicationDBContext
{
    private readonly IConfiguration _identityConfiguration;
    private readonly string _IdentityconnectionString;

    /// <summary>
    /// Initialize  
    /// </summary>
    /// <param name="configuration"></param>
    public IdentityApplicationDBContext(IConfiguration identityConfiguration)
    {
        _identityConfiguration = identityConfiguration;
        _IdentityconnectionString = _identityConfiguration.GetConnectionString("IdentityConnection");
    }

    /// <summary>
    /// CreateConnection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() => new SqlConnection(_IdentityconnectionString);
}
