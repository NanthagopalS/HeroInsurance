using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Admin.API.Helpers
{
    public class AppConfiguration
    {
        
        public static string GetConfiguration(string template)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
            return configuration[template];
        }
    }
}
