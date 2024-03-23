using MediatR;

namespace Application.Cqrs.Weathers.Queries.GetWeatherList;

public class GetWeatherListQuery : IRequest<WeatherListVm>
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }

    public int[]? SelectedYears { get; set; }
    public int[]? SelectedMonths { get; set; }
}