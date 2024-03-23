using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExcelParser;

/// <summary>
/// Парсер для чтения данных из Excel-файла с информацией о погоде.
/// </summary>
public class Parser
{
    /// <summary>
    /// Словарь для хранения методов парсинга значений различных типов.
    /// </summary>
    private readonly Dictionary<Type, MethodInfo> parseMethods = [];

    /// <summary>
    /// Конструктор класса Parser.
    /// Инициализирует словарь методами парсинга для типов данных, которые должны встретиться в файле.
    /// </summary>
    public Parser()
    {
        AddParseMethod<double>();
        AddParseMethod<int>();
        AddParseMethod<DateOnly>();
        AddParseMethod<TimeOnly>();
    }

    /// <summary>
    /// Добавляет метод парсинга для указанного типа данных в словарь <see cref="parseMethods"/>.
    /// </summary>
    /// <typeparam name="T">Тип данных.</typeparam>
    /// <returns>Метод парсинга.</returns>
    private MethodInfo? AddParseMethod<T>() where T : struct => AddParseMethod(typeof(T));

    /// <summary>
    /// Добавляет метод парсинга для указанного типа данных в словарь <see cref="parseMethods"/>.
    /// </summary>
    /// <param name="type">Тип данных.</param>
    /// <returns>Метод парсинга.</returns>
    private MethodInfo? AddParseMethod(Type type)
    {
        if (!type.IsValueType) return null;
        try
        {
            var method = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, [typeof(string), typeof(IFormatProvider)], null);
            if (method != null)
                parseMethods.Add(type, method);

            return method;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Парсит значение ячейки в указанный тип данных.
    /// </summary>
    /// <typeparam name="T">Тип данных для парсинга.</typeparam>
    /// <param name="row">Строка в Excel-файле.</param>
    /// <param name="cellIndex">Индекс ячейки.</param>
    /// <returns>Значение ячейки, сконвертированное в указанный тип данных.</returns>
    private T? ParseCellValue<T>(IRow row, int cellIndex) where T : struct
    {
        var cell = row.GetCell(cellIndex);
        if (cell == null || cell.CellType == CellType.Blank)
            return null;

        var cellValue = cell.ToString();
        if (string.IsNullOrWhiteSpace(cellValue))
            return null;

        try
        {
            if (!parseMethods.TryGetValue(typeof(T), out MethodInfo? parseMethod))
                parseMethod = AddParseMethod(typeof(T));

            if (parseMethod == null) return null;

            var type = typeof(T);
            var result = parseMethod.Invoke(null, [cellValue, null]);
            return (T?)result;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Парсит значение ячейки в строковый формат.
    /// </summary>
    /// <param name="row">Строка в Excel-файле.</param>
    /// <param name="cellIndex">Индекс ячейки.</param>
    /// <returns>Значение ячейки в строковом формате.</returns>
    private static string? ParseCellString(IRow row, int cellIndex)
    {
        var cell = row.GetCell(cellIndex);
        if (cell == null || cell.CellType == CellType.Blank)
            return null;

        var cellValue = cell.ToString();
        if (string.IsNullOrWhiteSpace(cellValue))
            return null;
        return cellValue;
    }

    /// <summary>
    /// Парсит данные о погоде из Excel-файла.
    /// </summary>
    /// <param name="filePath">Путь к Excel-файлу.</param>
    /// <returns>Список объектов WeatherData, содержащих информацию о погоде.</returns>
    public List<WeatherData> Parse(string filePath)
    {
        List<WeatherData> result = [];
        using (FileStream file = new(filePath, FileMode.Open, FileAccess.Read))
        {
            var workbook = new XSSFWorkbook(file);

            for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
            {
                ISheet sheet = workbook.GetSheetAt(sheetIndex);

                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    int col = 0;

                    if (row == null) continue;

                    var date = ParseCellValue<DateOnly>(row, col++);
                    if (!date.HasValue) continue;
                    var time = ParseCellValue<TimeOnly>(row, col++);
                    if (!time.HasValue) continue;
                    var temperature = ParseCellValue<double>(row, col++);
                    if (!temperature.HasValue) continue;

                    result.Add(new WeatherData
                    {
                        Date = date.Value,
                        Time = time.Value,
                        Temperature = temperature.Value,
                        RelativeHumidity = ParseCellValue<double>(row, col++),
                        DewPoint = ParseCellValue<double>(row, col++),
                        AtmosphericPressure = ParseCellValue<int>(row, col++),
                        WindDirection = ParseCellString(row, col++),
                        WindSpeed = ParseCellValue<double>(row, col++),
                        Cloudiness = ParseCellValue<double>(row, col++),
                        LowerLimitCloud = ParseCellValue<int>(row, col++),
                        Visibility = ParseCellValue<int>(row, col++),
                        WeatherPhenomena = ParseCellString(row, col++),
                    });
                }
            }
        }

        return result;
    }
}