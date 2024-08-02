using Insurance.Web.Abstraction;
using Insurance.Web.Implementation;
using Insurance.Web.Models;
using Serilog;

namespace Insurance.Web;

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

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.Configure<HeroConfig>(configuration.GetSection("Hero"));
        builder.Services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
        builder.Services.AddHttpClient<IInsuranceService, InsuranceService>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Hero:BaseURL"));
            client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}/{userId?}/{refnumber?}");

        app.Run();
    }
}