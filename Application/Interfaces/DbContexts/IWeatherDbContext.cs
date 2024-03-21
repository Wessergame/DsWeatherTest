using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.DbContexts;
public interface IWeatherDbContext : IDbContextBase
{
    DbSet<Weather> Weathers { get; set; }
}