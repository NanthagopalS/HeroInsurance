﻿using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace POSP.Persistence.Configuration;
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
