using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace QueueServices.Configuration
{
    public class ApplicationDBContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        /// <summary>
        /// Initialize  
        /// </summary>
        /// <param name="configuration"></param>
        public ApplicationDBContext()
        {
            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            _configuration = conf;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("SqlConnection").Value;
        }

        /// <summary>
        /// CreateConnection
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
