using Companies.API.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts;
using Tournament.Api.Extensions;
using Tournament.Core.Repositories;
using Tournament.Data.Data;
using Tournament.Data.Repositories;
using Tournaments.Services;

namespace Tournament.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TournamentContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("TournamentContext") 
                    ?? throw new InvalidOperationException("Connection string 'TournamentContext' not found.")));



            // Add services to the container.
            builder.Services.AddControllers(opt => opt.ReturnHttpNotAcceptable = true)
                .AddNewtonsoftJson();
            //    .AddXmlDataContractSerializerFormatters();

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(TournamentMappings));

            builder.Services.ConfigureServiceLayerServices();
            builder.Services.ConfigureRepositories();

            //builder.Services.AddScoped<IServiceManager, ServiceManager>();
            //builder.Services.AddScoped<IGameService, GameService>();
            //builder.Services.AddScoped<ITournamentDetailsService, TournamentDetailsService>();

            // services.AddScoped(provider => new Lazy<ICompanyService>(() => provider.GetRequiredService<ICompanyService>()));
            //builder.Services.AddLazy<ITournamentDetailsService>();
            //builder.Services.AddLazy<IGameService>();

            //builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
            //builder.Services.AddScoped<IGameRepository, GameRepository>();
            //builder.Services.AddScoped<ITournamentUoW, TournamentUoW>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                await app.SeedDataAsync();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
