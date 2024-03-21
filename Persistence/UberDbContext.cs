using Application.Interfaces.DbContexts;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence.EntityTypeConfigurations;

namespace Persistence;

public class UberDbContext(DbContextOptions<UberDbContext> options) : DbContext(options), IWeatherDbContext, IWindDirectionDbContext
{
    public DbSet<Weather> Weathers { get; set; } = null!;
    public DbSet<WindDirection> WindDirections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WeatherConfiguration());
        modelBuilder.ApplyConfiguration(new WindDirectionConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}