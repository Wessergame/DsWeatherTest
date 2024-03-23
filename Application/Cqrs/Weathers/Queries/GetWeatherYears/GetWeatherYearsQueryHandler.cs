using Application.Interfaces.DbContexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Cqrs.Weathers.Queries.GetWeatherYears;

public class GetWeatherYearsQueryHandler(IWeatherDbContext dbContext) : IRequestHandler<GetWeatherYearsQuery, WeatherYearsVm>
{
    public async Task<WeatherYearsVm> Handle(GetWeatherYearsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Weathers.Select(w => w.Date.Year).Distinct().ToListAsync(cancellationToken);

        return new WeatherYearsVm
        {
            Years = await query
        };
    }
}