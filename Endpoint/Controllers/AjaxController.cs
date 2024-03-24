using Application.Cqrs.Weathers.Queries.GetWeatherList;
using Endpoint.Adapter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Endpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AjaxController(ExcelToSqlAdapter adapter, IMediator mediator) : ControllerBase
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

        [HttpPost]
        [Route("UploadWeatherArchives")]
        public async Task<IActionResult> UploadWeatherArchives([FromForm] List<IFormFile> files)
        {
            if (files.Count == 0) return Ok();

            try
            {
                var failedFiles = await adapter.RunParallel(files);

                if (failedFiles.Count == files.Count)
                    return StatusCode(StatusCodes.Status400BadRequest, $"Файлы не удалось обработать.");

                if (failedFiles.Count > 0)
                    return Ok("Архивы были успешно загружены, но некоторые файлы не удалось обработать: " + string.Join(", ", failedFiles));

                return Ok("Архивы были успешно загружены");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            finally
            {
                adapter.Dispose();
            }
        }
    }
}
