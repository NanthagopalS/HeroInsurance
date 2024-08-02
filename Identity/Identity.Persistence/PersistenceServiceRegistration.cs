using Identity.Core.Contracts.Persistence;
using Identity.Persistence.Configuration;
using Identity.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Implementation;

namespace Identity.Persistence;
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
        services.AddSingleton<SignzyLogsDBContext>();
		services.AddSingleton<LogsDBContext>();

		services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IBannerRepository, BannerRepository>();
        services.TryAddScoped<IAuthenticateRepository, AuthenticateRepository>();
        services.TryAddScoped<IMongoDBService, MongoDBService>();
        services.TryAddScoped<ILogService, LogService>();
		services.TryAddScoped<ILogRepository, LogRepository>();

		return services;
    }
}
