using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Cqrs.Weathers.Queries.GetWeatherYears;

public class GetWeatherYearsQuery : IRequest<WeatherYearsVm>;