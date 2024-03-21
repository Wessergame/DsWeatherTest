using Application.Interfaces.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration["ConnectionStringSql"];
        services.AddDbContext<UberDbContext>(option => option.UseNpgsql(connection));

        services.AddScoped<IWeatherDbContext>(provider => provider.GetService<UberDbContext>()!);
        services.AddScoped<IWindDirectionDbContext>(provider => provider.GetService<UberDbContext>()!);

        return services;
    }
}
