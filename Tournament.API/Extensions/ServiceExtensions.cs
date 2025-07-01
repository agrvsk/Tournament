
//using Microsoft.Identity.Client.Extensibility;
using Service.Contracts;
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

            // services.AddScoped(provider => new Lazy<ICompanyService>(() => provider.GetRequiredService<ICompanyService>()));
            services.AddLazy<ITournamentDetailsService>();
            services.AddLazy<IGameService>();
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<ITournamentRepository, TournamentRepository>();

            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ITournamentUoW, TournamentUoW>();

            services.AddLazy<IGameRepository>();
            services.AddLazy<ITournamentRepository>();
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
