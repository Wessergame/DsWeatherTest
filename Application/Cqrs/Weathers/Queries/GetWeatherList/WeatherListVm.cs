using System.Collections.Generic;

namespace Application.Cqrs.Weathers.Queries.GetWeatherList;

public class WeatherListVm
{
    public int TotalData { get; set; }
    public required List<WeatherListLookupDto> Weathers { get; set; }
}