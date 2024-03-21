using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.DbContexts;
public interface IDbContextBase
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}