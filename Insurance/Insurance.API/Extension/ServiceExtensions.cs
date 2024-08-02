using Insurance.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Hero.Api.Extension
{
    /// <summary>
    /// Added extension for Swagger documentation and API Versioning
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceExtensions
    {
        /// <summary>
        /// Added Swagger version and Security definition
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static void AddSwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "V1",
                    Title = "Hero API",
                });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Input your Bearer token to access this API",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        }, new List<string>()
                    },
                });
                swagger.MapType<DateTime>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new CustomOpenApiDateTime(DateTime.Now)
                });
                //Get api ep decsription
                var currentAssembly = Assembly.GetExecutingAssembly();

                //Gets the view model description
                var xmlDocs = currentAssembly.GetReferencedAssemblies()
                .Union(new AssemblyName[] { currentAssembly.GetName() })
                .Select(a =>
                {
                    return Path.Combine(Path.GetDirectoryName(currentAssembly.Location), $"{a.Name}.xml");
                })
                .Where(f => File.Exists(f)).ToArray();
                foreach (var item in xmlDocs)
                {
                    swagger.IncludeXmlComments(item);
                }

            });
            services.AddSwaggerGenNewtonsoftSupport();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Added API versioning and default version
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public static void AddApiVersioningExtension(this IServiceCollection services)
        {
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
