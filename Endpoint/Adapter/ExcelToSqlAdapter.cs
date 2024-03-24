using Domain;
using ExcelParser;
using Microsoft.AspNetCore.Http;
using Persistence;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Endpoint.Adapter;

public class ExcelToSqlAdapter(UberDbContext dbContext) : IDisposable
{
    private readonly List<WindDirection> windDirections = [.. dbContext.WindDirections];
    private readonly List<WindDirection> newWindDirections = [];

    private readonly string tempFolder = Path.Combine(Path.GetTempPath(), "WeatherArchives");

    public void Dispose()
    {
        if (Directory.Exists(tempFolder))
            Directory.Delete(tempFolder, true);
    }

    private async Task<List<string>> SaveFiles(List<IFormFile> files, CancellationToken cancellationToken)
    {
        var filePaths = new List<string>();

        foreach (var file in files)
        {
            var filePath = Path.Combine(tempFolder, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            filePaths.Add(filePath);
        }

        return filePaths;
    }

    public async Task<List<string>> RunParallel(List<IFormFile> files, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(tempFolder))
            Directory.CreateDirectory(tempFolder);

        var filePaths = await SaveFiles(files, cancellationToken);

        var parseTasks = new List<Task<List<WeatherData>>>();

        foreach (var filePath in filePaths)
        {
            var parser = new Parser();
            parseTasks.Add(Task.Run(() => parser.Parse(filePath)));
        }

        await Task.WhenAll(parseTasks);

        var failedFiles = new List<string>();

        for (int i = 0; i < filePaths.Count; i++)
        {
            var filePath = filePaths[i];
            var parseResult = parseTasks[i].Result;

            if (parseResult == null || parseResult.Count == 0)
                failedFiles.Add(Path.GetFileName(filePath));
            else
                await Run(parseResult, cancellationToken);
        }

        return failedFiles;
    }

    public async Task Run(List<WeatherData> excelData, CancellationToken cancellationToken = default)
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