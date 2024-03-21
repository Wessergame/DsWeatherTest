using System;

namespace ExcelParser;

/// <summary>
/// Представление данных о погоде.
/// </summary>
public class WeatherData
{
    /// <summary>
    /// Дата.
    /// </summary>
    public DateOnly Date { get; set; }
    /// <summary>
    /// Время.
    /// </summary>
    public TimeOnly Time { get; set; }
    /// <summary>
    /// Температура воздуха (T).
    /// </summary>
    public double Temperature { get; set; }
    /// <summary>
    /// Относительная влажность воздуха, %.
    /// </summary>
    public double? RelativeHumidity { get; set; }
    /// <summary>
    /// Точка росы (TD).
    /// </summary>
    public double? DewPoint { get; set; }
    /// <summary>
    /// Атмосферное давление, мм рт.ст..
    /// </summary>
    public double? AtmosphericPressure { get; set; }
    /// <summary>
    /// Направление ветра.
    /// </summary>
    public string? WindDirection { get; set; }
    /// <summary>
    /// Скорость ветра, м/с.
    /// </summary>
    public double? WindSpeed { get; set; }
    /// <summary>
    /// Облачность, %
    /// </summary>
    public double? Cloudiness { get; set; }
    /// <summary>
    /// Нижняя граница облачности, м (h).
    /// </summary>
    public int? LowerLimitCloud { get; set; }
    /// <summary>
    /// Горизонтальная видимость, км (VV).
    /// </summary>
    public int? Visibility { get; set; }
    /// <summary>
    /// Погодные явления.
    /// </summary>
    public string? WeatherPhenomena { get; set; }
}