using System;
using System.Collections.Generic;

namespace Domain;
public class Weather
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public double Temperature { get; set; }
    public double? RelativeHumidity { get; set; }
    public double? DewPoint { get; set; }
    public double? AtmosphericPressure { get; set; }
    public double? WindSpeed { get; set; }
    public double? Cloudiness { get; set; }
    public int? LowerLimitCloud { get; set; }
    public int? Visibility { get; set; }
    public string? WeatherPhenomena { get; set; }

    public List<WindDirection>? WindDirections { get; set; }
}