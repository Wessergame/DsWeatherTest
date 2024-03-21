using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces.DbContexts;
public interface IWindDirectionDbContext : IDbContextBase
{
    DbSet<WindDirection> WindDirections { get; set; }
}