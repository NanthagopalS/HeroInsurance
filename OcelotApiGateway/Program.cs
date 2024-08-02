using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers();

builder.Configuration.AddJsonFile($"ocelot.json", optional: true, reloadOnChange: true);
builder.Services.AddOcelot();
builder.Services.AddSwaggerForOcelot(configuration);

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
    //options.Events = new JwtBearerEvents()
    //{
    //    OnChallenge = async context =>
    //    {
    //        context.HandleResponse();
    //        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //        context.Response.ContentType = "application/problem+json";
    //        var problemResponse = Result.CreateUnAuthorizedError("UnAuthorized");
    //        await context.Response.WriteAsync(JsonSerializer.Serialize(problemResponse));
    //    }
    //};
});

var app = builder.Build();

app.UseSwaggerForOcelotUI();

app.UseRouting();

//app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

app.MapControllers();

app.UseOcelot().Wait();

app.Run();
