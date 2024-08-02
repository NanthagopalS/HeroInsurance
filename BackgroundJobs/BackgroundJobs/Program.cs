using BackgroundJobs.Configuration;
using BackgroundJobs.Helpers;
using BackgroundJobs.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using BackgroundJobs.Repository.Repository.Implementation;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace BackgroundJobs;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", fileSizeLimitBytes: 10000000, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        ConfigurationManager configuration = builder.Configuration;

        builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", fileSizeLimitBytes: 10000000, rollOnFileSizeLimit: true, rollingInterval: RollingInterval.Day));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("DBConnection")));
        builder.Services.AddHangfireServer();

        builder.Services.Configure<HeroConfig>(configuration.GetSection("Hero"));
        builder.Services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
        builder.Services.AddHttpClient<IICICIRepository, ICICIRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<IGoDigitRepository, GoDigitRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<IBajajRepository, BajajRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<IHDFCRepository, HDFCRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<ICholaRepository, CholaRepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<IIFFCORepository, IFFCORepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        builder.Services.AddHttpClient<ITATARepository, TATARepository>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(60);
        });
        //builder.Services.AddHttpClient<IQuoteRepository, QuoteRepository>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
        //    client.Timeout = TimeSpan.FromSeconds(60);
        //});

        builder.Services.TryAddSingleton<ApplicationDBContext>();
        builder.Services.TryAddSingleton<IdentityApplicationDBContext>();
        builder.Services.AddTransient<IQuoteRepository, QuoteRepository>();  
        var app = builder.Build();

        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler(c => c.Run(async context =>
        {
            await HttpGlobalExceptionHandler.ExceptionHandler(context, app);
        }));

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Hangfire",
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter{
                    User = configuration.GetSection("HangfireSettings:UserName").Value,
                    Pass = configuration.GetSection("HangfireSettings:Password").Value
                }
            }
        });

        app.MapControllers();

        app.Run();
    }
}
