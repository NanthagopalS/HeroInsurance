using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Insurance.API.HealthCheck
{
    /// <summary>
    /// Health check routebuilder extension
    /// </summary>
    public static class HealthCheckRouteBuilderExtension
    {
        /// <summary>
        /// configures the healthcheck endpoint
        /// </summary>
        /// <param name="endpoints">IEndpointRouteBuilder</param>
        /// <returns>IEndpointRouteBuilder</returns>
        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResultStatusCodes =
                    {
                        [HealthStatus.Healthy]=StatusCodes.Status200OK,
                        [HealthStatus.Degraded]=StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy]=StatusCodes.Status503ServiceUnavailable
                    },
                ResponseWriter = HealthCheckResponses.WriteHealthCheckReadyResponse
            });
            return endpoints;
        }


    }
}
