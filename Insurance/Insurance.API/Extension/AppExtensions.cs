using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Diagnostics.CodeAnalysis;

namespace Insurance.API.Extension
{
    /// <summary>
    /// Swagger and Middleware extension 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AppExtensions
    {
        /// <summary>
        /// Swagger version and URL registration
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="provider">IApiVersionDescriptionProvider</param>
        public static void UseSwaggerExtension(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
                options.DefaultModelsExpandDepth(-1);
            });
        }
    }
}
