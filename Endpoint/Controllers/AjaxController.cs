using Application.Cqrs.Weathers.Queries.GetWeatherList;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AjaxController(IMediator mediator) : ControllerBase
    {
        public class WeatherDataFilters
        {
            public int[]? SelectedMonths { get; set; }
            public int[]? SelectedYears { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }

        public class WeatherDataResponse
        {
            public int TotalData { get; set; }
            public required List<WeatherListLookupDto> WeatherData { get; set; }
        }

        [HttpPost]
        [Route("GetWeatherData")]
        public async Task<IActionResult> GetWeatherData([FromBody] WeatherDataFilters options)
        {
            var data = await mediator.Send(new GetWeatherListQuery
            {
                Page = options.PageNumber < 1 ? 1 : options.PageNumber,
                PageSize = options.PageSize < 1 ? 1 : options.PageSize,
                SelectedMonths = options.SelectedMonths,
                SelectedYears = options.SelectedYears,
            });

            return Ok(data);
        }
    }
}
