using Hero.Api.Extension;
using Insurance.API.Common;
using Insurance.API.Extension;
using Insurance.API.HealthCheck;
using Insurance.API.Helpers;
using Insurance.Core;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Helpers;
using Insurance.Domain.Bajaj;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Magma;
using Insurance.Domain.Oriental;
using Insurance.Domain.Reliance;
using Insurance.Domain.TATA;
using Insurance.Domain.UnitedIndia;
using Insurance.Persistence;
using Insurance.Persistence.ICIntegration.Abstraction;
using Insurance.Persistence.ICIntegration.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Refit;
using Serilog;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Implementation;
using ThirdPartyUtilities.Models;
using ThirdPartyUtilities.Models.Email;
using ThirdPartyUtilities.Models.JWT;
using ThirdPartyUtilities.Models.MongoDB;
using ThirdPartyUtilities.Models.Signzy;
using ThirdPartyUtilities.Models.SMS;

Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Hour)
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Hour));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();


//Adding CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCorsPolicy", builder =>
    {
        var allowedOriginAddress = configuration.GetValue<string>("AllowedCorsUrls");
        var addresses = allowedOriginAddress.Split(';');
        builder.WithOrigins(addresses)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices();

//ThirdParty configuration
ThirdPartyConfiguration(builder, configuration);

//IC Configuration
ICConfiguration(builder, configuration);

builder.Services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["TokenSettings:Audience"],
        ValidIssuer = configuration["TokenSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenSettings:SigningKey"]))
    };
    options.Events = new JwtBearerEvents()
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";
            var problemResponse = ThirdPartyUtilities.Helpers.Result.CreateUnAuthorizedError("UnAuthorized");
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemResponse));
        }
    };
});

builder.Services.AddSingleton<ICustomUtility, CustomUtility>();
builder.Services.AddHttpContextAccessor();

builder.Services.TryAddScoped<IApplicationClaims, ApplicationClaims>();
builder.Services.TryAddScoped<IInsurerCheck, InsurerCheck>();

//HealthCheck
builder.Services.AddApihealthCheck(configuration);

builder.Services.AddScoped<ResponseCaptureFilter>();

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(c => c.Run(async context =>
{
    await HttpGlobalExceptionHandler.ExceptionHandler(context, app);
}));

app.Use(async (context, next) =>
{
	context.Request.EnableBuffering();
	await next();
});

app.UseHttpsRedirection();

app.UseCors("ApiCorsPolicy");

app.UseSwaggerExtension(provider);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultHealthChecks();

app.Run();

static void ICConfiguration(WebApplicationBuilder builder, ConfigurationManager configuration)
{
    builder.Services.AddHttpClient<IGodigitService, GoDigitService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("GoDigit:BaseURL"));
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(configuration.GetValue<string>("GoDigit:AuthCode")));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("GoDigit:Duration"));

    });
    builder.Services.AddHttpClient<IICICIService, ICICIService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("ICICI:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("ICICI:Duration"));
    });

    builder.Services.AddHttpClient<IBajajService, BajajService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("Bajaj:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("Bajaj:Duration"));
    });
    builder.Services.AddHttpClient<IHDFCService, HDFCService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("HDFC:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("HDFC:Duration"));

    });

    builder.Services.AddHttpClient<ICholaService, CholaService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("Chola:BaseURL"));
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(configuration.GetValue<string>("Chola:AuthCode")));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("Chola:Duration"));
    });

    builder.Services.AddHttpClient<IRelianceService, RelianceService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("Reliance:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
    });
    builder.Services.AddHttpClient<ITATAService, TATAService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("TATA:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
    });
    builder.Services.AddHttpClient<IMagmaService, MagmaService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("Magma:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
    });

    builder.Services.AddHttpClient<IIFFCOService, IFFCOService>(client =>{
        client.BaseAddress = new Uri(configuration.GetValue<string>("IFFCO:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
    });
    builder.Services.AddHttpClient<IOrientalService, OrientalService>(client => {
        client.BaseAddress = new Uri(configuration.GetValue<string>("Oriental:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("RequestTimeout:Duration"));
    });
    builder.Services.AddHttpClient<IUnitedIndiaService, UnitedIndiaService>(client => {
        client.BaseAddress = new Uri(configuration.GetValue<string>("UnitedIndia:BaseURL"));
        client.Timeout = TimeSpan.FromSeconds(configuration.GetValue<double>("UnitedIndia:Duration"));
    });
    builder.Services.Configure<GoDigitConfig>(configuration.GetSection("GoDigit"));
    builder.Services.Configure<BajajConfig>(configuration.GetSection("Bajaj"));
    builder.Services.Configure<ICICIConfig>(configuration.GetSection("ICICI"));
    builder.Services.Configure<HDFCConfig>(configuration.GetSection("HDFC"));
    builder.Services.Configure<CholaConfig>(configuration.GetSection("Chola"));
    builder.Services.Configure<RelianceConfig>(configuration.GetSection("Reliance"));
    builder.Services.Configure<IFFCOConfig>(configuration.GetSection("IFFCO"));
    builder.Services.Configure<TATAConfig>(configuration.GetSection("TATA"));
    builder.Services.Configure<MagmaConfig>(configuration.GetSection("Magma"));
    builder.Services.Configure<OrientalConfig>(configuration.GetSection("Oriental"));
    builder.Services.Configure<UnitedIndiaConfig>(configuration.GetSection("UnitedIndia"));
    builder.Services.Configure<PolicyTypeConfig>(configuration.GetSection("PolicyType"));
    builder.Services.Configure<VehicleTypeConfig>(configuration.GetSection("VehicleType"));
    builder.Services.Configure<MongodbConnection>(configuration.GetSection("MongoDbConnection"));
    builder.Services.Configure<InsurerIdConfig>(configuration.GetSection("InsurerId"));
    builder.Services.Configure<LogoConfig>(configuration.GetSection("Logo"));
    builder.Services.Configure<SignzyDBConnectModel>(configuration.GetSection("ConnectionStrings"));
    builder.Services.Configure<ShareLinkFrontEndURL>(configuration.GetSection("ShareLink"));
}

static void ThirdPartyConfiguration(WebApplicationBuilder builder, ConfigurationManager configuration)
{
    builder.Services.AddHttpClient<ISignzyService, SignzyService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("SignzyConfig:BaseURL"));
        client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        client.DefaultRequestHeaders.Add("Accept", "*/*");
    });
    builder.Services.AddSingleton<ICustomUtility, CustomUtility>();
    builder.Services.AddHttpClient<ISmsService, SmsService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("SMSConfig:BaseURL"));
    });

    builder.Services.AddHttpClient<IEmailService, EmailService>(client =>
    {
        client.BaseAddress = new Uri(configuration.GetValue<string>("EmailConfig:BaseURL"));
    });

    //ThirdParty config
    builder.Services.Configure<SignzyConfig>(configuration.GetSection("SignzyConfig"));
    builder.Services.Configure<SMSConfig>(configuration.GetSection("SMSConfig"));
    builder.Services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));
}