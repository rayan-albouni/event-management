using EventManagement.Api.Authorization;
using EventManagement.Application.Interfaces;
using EventManagement.Application.Services;
using EventManagement.Domain.Enums;
using EventManagement.Domain.Interfaces;
using EventManagement.Infrastructure.Data;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EventManagement.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration.");

        services.AddDbContext<EventManagementDbContext>(options =>
            options.UseSqlite(connectionString));

    services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IRegistrationService, RegistrationService>();

        return services;
    }

    public static IServiceCollection AddRoleBasedAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.CreateEvent, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.CreateEvent)));

            options.AddPolicy(PolicyNames.ReadEvent, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.ReadEvent)));

            options.AddPolicy(PolicyNames.UpdateEvent, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.UpdateEvent)));

            options.AddPolicy(PolicyNames.DeleteEvent, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.DeleteEvent)));

            options.AddPolicy(PolicyNames.CreateRegistration, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.CreateRegistration)));

            options.AddPolicy(PolicyNames.ReadRegistration, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.ReadRegistration)));

            options.AddPolicy(PolicyNames.DeleteRegistration, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.DeleteRegistration)));

            options.AddPolicy(PolicyNames.ReadEventRegistrations, policy =>
                policy.Requirements.Add(new PermissionRequirement(Permission.ReadEventRegistrations)));
        });

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found in configuration.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found in configuration.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not found in configuration.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
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
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddCorsService(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
        });
        return services;
    }
}
