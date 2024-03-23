using MediatR;

namespace Application.Cqrs.Weathers.Queries.GetWeatherYears;

public class GetWeatherYearsQuery : IRequest<WeatherYearsVm>;