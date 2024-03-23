using Application.Interfaces.DbContexts;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Cqrs.Weathers.Queries.GetWeatherList;

public class GetWeatherListQueryHandler(IWeatherDbContext dbContext) : IRequestHandler<GetWeatherListQuery, WeatherListVm>
{
    public async Task<WeatherListVm> Handle(GetWeatherListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Weather> query = dbContext.Weathers
            .Where(w => (request.SelectedMonths == null || request.SelectedMonths.Length == 0 || request.SelectedMonths.Contains(w.Date.Month))
            && (request.SelectedYears == null || request.SelectedYears.Length == 0 || request.SelectedYears.Contains(w.Date.Year)))
            .OrderBy(w => w.Date);

        var countData = await query.CountAsync(cancellationToken);

        if (request is { PageSize: not null, Page: not null })
            query = query.Skip((request.Page.Value - 1) * request.PageSize.Value).Take(request.PageSize.Value);

        var result = await query
            .Include(w => w.WindDirections)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new WeatherListVm
        {
            TotalData = countData,
            Weathers = result.Select(w => new WeatherListLookupDto
            {
                Date = w.Date,
                Time = w.Time,
                Temperature = w.Temperature,
                RelativeHumidity = w.RelativeHumidity,
                DewPoint = w.DewPoint,
                AtmosphericPressure = w.AtmosphericPressure,
                WindSpeed = w.WindSpeed,
                Cloudiness = w.Cloudiness,
                LowerLimitCloud = w.LowerLimitCloud,
                Visibility = w.Visibility,
                WeatherPhenomena = w.WeatherPhenomena,
                WindDirections = w.WindDirections?.Select(d => d.Direction).ToList(),
            }).ToList()
        };
    }
}