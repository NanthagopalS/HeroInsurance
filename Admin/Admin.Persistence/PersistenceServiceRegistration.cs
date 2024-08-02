using Admin.Core.Contracts.Persistence;
using Admin.Persistence.Configuration;
using Admin.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Implementation;

namespace Admin.Persistence;
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
        services.AddSingleton<IdentityApplicationDBContext>();
        services.AddSingleton<MongoDBContext>();
        services.AddSingleton<SignzyLogsDBContext>();
		services.AddSingleton<LogsDBContext>();

		services.TryAddScoped<IMongoDBService, MongoDBService>();
        services.TryAddScoped<IUserRepository, UserRepository>();
        services.TryAddScoped<IBannerRepository, BannerRepository>();
        services.TryAddScoped<IHelpAndSupportRepository, HelAndSupportRepository>();
        services.TryAddScoped<INotificationRepository, NotificationRepository>();
        services.TryAddScoped<ITicketManagementRepository, TicketManagementRepository>();
        services.TryAddScoped<ILogService, LogService>();
        services.TryAddScoped<IMmvRepository, MmvRepository>();
        services.TryAddScoped<ILogRepository, LogRepository>();
        return services;
    }
}
