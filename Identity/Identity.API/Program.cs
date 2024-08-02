using Identity.Api.Extension;
using Identity.API.common;
using Identity.API.Extension;
using Identity.API.Helpers;
using Identity.Core;
using Identity.Core.Contracts.Common;
using Identity.Core.Helpers;
using Identity.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<FileUploadFilter>();
});

builder.Services.AddSwaggerExtension();
builder.Services.AddApiVersioningExtension();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueCountLimit = int.MaxValue;
});
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

builder.Services.Configure<SignzyConfig>(configuration.GetSection("SignzyConfig"));
builder.Services.Configure<SMSConfig>(configuration.GetSection("SMSConfig"));
builder.Services.Configure<EmailConfig>(configuration.GetSection("EmailConfig"));
builder.Services.Configure<TokenSettings>(configuration.GetSection("TokenSettings"));
builder.Services.Configure<MongodbConnection>(configuration.GetSection("MongoDbConnection"));
builder.Services.Configure<SignzyDBConnectModel>(configuration.GetSection("ConnectionStrings"));


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
            var problemResponse = Result.CreateUnAuthorizedError("UnAuthorized");
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemResponse));
        }
    };
});

builder.Services.AddHttpContextAccessor();

builder.Services.TryAddScoped<IApplicationClaims, ApplicationClaims>();

builder.Services.AddScoped<ResponseCaptureFilter>();

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler((Action<IApplicationBuilder>)(c => c.Run((RequestDelegate)(async context =>
{
    await HttpGlobalExceptionHandler.ExceptionHandler(context, app);
}))));

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

app.Run();