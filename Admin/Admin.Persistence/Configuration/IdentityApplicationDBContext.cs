using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Admin.Persistence.Configuration;

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
        _IdentityconnectionString = _identityConfiguration.GetConnectionString("IdentitySqlConnection");
    }

    /// <summary>
    /// CreateConnection
    /// </summary>
    /// <returns></returns>
    public IDbConnection CreateConnection() => new SqlConnection(_IdentityconnectionString);
}
