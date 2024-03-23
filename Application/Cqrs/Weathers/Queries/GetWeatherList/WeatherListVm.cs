using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cqrs.Weathers.Queries.GetWeatherList;

public class WeatherListVm
{
    public int TotalData { get; set; }
    public required List<WeatherListLookupDto> Weathers { get; set; }
}