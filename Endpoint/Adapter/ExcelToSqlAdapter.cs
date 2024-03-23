using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endpoint.Adapter;

public class ExcelToSqlAdapter(UberDbContext dbContext)
{
    private readonly List<WindDirection> windDirections = [.. dbContext.WindDirections];
    private readonly List<WindDirection> newWindDirections = [];
    public async Task Run(List<ExcelParser.WeatherData> excelData, CancellationToken cancellationToken = default)
    {
        List<Weather> result = [];
        foreach (var excel in excelData)
        {
            List<WindDirection> directions = [];

            if (excel.WindDirection != null)
            {
                foreach (var directionString in excel.WindDirection.Split(",",
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var direction = windDirections.FirstOrDefault(wd => wd.Direction.Equals(directionString, StringComparison.CurrentCultureIgnoreCase))
                        ?? newWindDirections.FirstOrDefault(wd => wd.Direction.Equals(directionString, StringComparison.CurrentCultureIgnoreCase));

                    if (direction == null)
                    {
                        direction = new WindDirection
                        {
                            Direction = directionString
                        };
                        newWindDirections.Add(direction);
                    }

                    directions.Add(direction);
                }
            }

            result.Add(new Weather
            {
                Date = excel.Date,
                Time = excel.Time,
                Temperature = excel.Temperature,
                RelativeHumidity = excel.RelativeHumidity,
                DewPoint = excel.DewPoint,
                AtmosphericPressure = excel.AtmosphericPressure,
                WindSpeed = excel.WindSpeed,
                Cloudiness = excel.Cloudiness,
                LowerLimitCloud = excel.LowerLimitCloud,
                Visibility = excel.Visibility,
                WeatherPhenomena = excel.WeatherPhenomena,
                WindDirections = directions
            });
        }

        dbContext.Weathers.AddRange(result);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}