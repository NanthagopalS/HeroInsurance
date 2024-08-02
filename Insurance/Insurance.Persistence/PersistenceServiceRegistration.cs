using Insurance.Core.Contracts.Persistence;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Insurance.Persistence.ICIntegration.Implementation;
using Insurance.Persistence.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Implementation;

namespace Insurance.Persistence;
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
        services.AddSingleton<LogsDBContext>();
        services.AddSingleton<SignzyLogsDBContext>();

        services.TryAddScoped<IInsuranceMasterRepository, InsuranceMasterRepository>();
        services.TryAddScoped<IBajajRepository, BajajRepository>();
        services.TryAddScoped<IGoDigitRepository, GoDigitRepository>();
        services.TryAddScoped<IICICIRepository, ICICIRepository>();
        services.TryAddScoped<ICholaRepository, CholaRepository>();
        services.TryAddScoped<IHDFCRepository, HDFCRepository>();
        services.TryAddScoped<IRelianceRepository, RelianceRepository>();
        services.TryAddScoped<ITATARepository, TATARepository>();
        services.TryAddScoped<IQuoteRepository, QuoteRepository>();
        services.TryAddScoped<IMagmaRepository, MagmaRepository>();
        services.TryAddScoped<IMongoDBService, MongoDBService>();
        services.TryAddScoped<ILeadsRepository, LeadsRepository>();
        services.TryAddScoped<ICustomerRepository, CustomerRepository>();
        services.TryAddScoped<IReportAndMISRepository, AllReportAndMISRepository>();
        services.TryAddScoped<ICommonService, CommonService>();
        services.TryAddScoped<ILogService, LogService>();
        services.TryAddScoped<IShareLinkRepository, ShareLinkRepository>();
        services.TryAddScoped<IIFFCORepository, IFFCORepository>();
        services.TryAddScoped<IInsuranceReportRepository, InsuranceReportRepository>();
        services.TryAddScoped<IManualPolicyRepository, ManualPolicyRepository>();
        services.TryAddScoped<IOrientalRepository, OrientalRepository>();
        services.TryAddScoped<ICommercialMasterRepository, CommercialMasterRepository>();
		services.TryAddScoped<ILogsRepository, LogsRepository>();
        services.TryAddScoped<IUnitedIndiaRepository, UnitedIndiaRepository>();
        return services;
    }
}
