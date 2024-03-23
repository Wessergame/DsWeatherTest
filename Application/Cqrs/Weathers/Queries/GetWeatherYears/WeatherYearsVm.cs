using System.Collections.Generic;

namespace Application.Cqrs.Weathers.Queries.GetWeatherYears;

public class WeatherYearsVm
{
    public required List<int> Years { get; set; }
}