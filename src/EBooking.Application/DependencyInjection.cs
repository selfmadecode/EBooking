using EBooking.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EBooking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Event Booking Management API",
                Version = "v1",
                Description = "A RESTful API for managing event bookings with integrated wallet system",
                Contact = new OpenApiContact { Name = "Developer", Email = "dev@eventbooking.com" },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://en.wikipedia.org/wiki/MIT_Lincense")
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using Bearer scheme. \r\n\r\n" +
                "Enter 'Bearer' [space] and then your token in the input below.\r\n\r\n" +
                "Example: \"Bearer 123456\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }

            });
            c.DescribeAllParametersInCamelCase();
        });

        return services;
    }
}
