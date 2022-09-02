using System.Reflection;

using Microsoft.OpenApi.Models;

namespace Downcast.Authentication.API.Config;

public static class SwaggerConfigExtensions
{
    private static string XmlCommentsFilePath => $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, XmlCommentsFilePath));
            options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }
}