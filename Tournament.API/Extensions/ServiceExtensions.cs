
//using Microsoft.Identity.Client.Extensibility;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service.Contracts;
using Tournament.Core.Configuration;
using Tournament.Core.Repositories;
using Tournament.Data.Repositories;
using Tournaments.Services;

namespace Tournament.Api.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(builder =>
        {
            builder.AddPolicy("AllowAll", p =>
            p.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
        });
    }

    public static void ConfigureServiceLayerServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<ITournamentDetailsService, TournamentDetailsService>();
        services.AddScoped<IAuthService, AuthService>();

        // services.AddScoped(provider => new Lazy<ICompanyService>(() => provider.GetRequiredService<ICompanyService>()));
        services.AddLazy<ITournamentDetailsService>();
        services.AddLazy<IGameService>();
        services.AddLazy<IAuthService>();
    }

    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ITournamentUoW, TournamentUoW>();

        services.AddLazy<IUserRepository>();
        services.AddLazy<IGameRepository>();
        services.AddLazy<ITournamentRepository>();
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        ArgumentNullException.ThrowIfNull(nameof(jwtSettings));

        var secretKey = configuration["secretkey"];
        ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));

        var jwtConfig = new JwtConfiguration();
        jwtSettings.Bind(jwtConfig);

        services.Configure<JwtConfiguration>(options =>
        {
            options.Issuer = jwtConfig.Issuer;
            options.Audience = jwtConfig.Audience;
            options.Expires = jwtConfig.Expires;
            options.SecretKey = secretKey;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = true
            };
        });
    }
    // Swagger med tokens
    public static void ConfigureOpenApi(this IServiceCollection services) =>
            services.AddEndpointsApiExplorer()
       .AddSwaggerGen(setup =>
       {
           setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
           {
               In = ParameterLocation.Header,
               Description = "Place to add JWT with Bearer",
               Name = "Authorization",
               Type = SecuritySchemeType.Http,
               Scheme = "Bearer"
           });

           setup.AddSecurityRequirement(new OpenApiSecurityRequirement
           {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
           });
       });





}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
    {
        return services.AddScoped(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
    }
}
