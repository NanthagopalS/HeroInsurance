using Insurance.Core.Features.GoDigit.Command.CKYC;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Insurance.API.HealthCheck
{
    public static class HealthCheckServiceCollectionExtension
    {
        /// <summary>
        /// Extends IServicecollection on API health check
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddApihealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration["ConnectionStrings:SqlConnection"], name: "HeroInsurance", failureStatus: HealthStatus.Unhealthy)
                .AddSqlServer(configuration["ConnectionStrings:IdentitySqlConnection"], name: "HeroIdentity", failureStatus: HealthStatus.Unhealthy);
            return services;
        }
    }
}
