
//using Microsoft.Identity.Client.Extensibility;
using System.Text;
using Domain.Models.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Services.Contracts;
using Tournament.Core.Repositories;
using Tournament.Data.Repositories;
using Tournaments.Services;

namespace Companies.API.Extensions
{
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
            services.AddScoped<ITournamentUoW, TournamentUoW>();

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





    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLazy<TService>(this IServiceCollection services) where TService : class
        {
            return services.AddScoped(provider => new Lazy<TService>(() => provider.GetRequiredService<TService>()));
        }
    }


}
