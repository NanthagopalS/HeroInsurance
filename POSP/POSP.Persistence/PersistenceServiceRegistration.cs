using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using POSP.Core.Contracts.Persistence;
using POSP.Persistence.Configuration;
using POSP.Persistence.Repository;
using System.Diagnostics.CodeAnalysis;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Implementation;

namespace POSP.Persistence;
/// <summary>
/// Persistence Service Registration
/// </summary>
/// 
[ExcludeFromCodeCoverage]
public static class PersistenceServiceRegistration
{
    /// <summary>
    /// AddPersistenceServices
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
        services.AddSingleton<ApplicationDBContext>();

        services.AddSingleton<MongoDBContext>();
		services.AddSingleton<LogsDBContext>();
		services.TryAddScoped<IMongoDBService, MongoDBService>();


        services.TryAddScoped<IPOSPRepository, POSPRepository>();
        services.TryAddScoped<IPOSPReportRepository, POSPReportRepositoty>();
        services.TryAddScoped<IPOSPMigrationRepository, POSPMigrationRepository>();
		services.TryAddScoped<ILogsRepository, LogsRepository>();

		return services;
    }
}
