using Application.Cqrs.Weathers.Queries.GetWeatherYears;
using Endpoint.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Endpoint.Controllers;

public class HomeController(ILogger<HomeController> logger, IMediator mediator) : Controller
{

    public async Task<IActionResult> Show()
    {
        ViewBag.Months = Enumerable.Range(1, 12)
            .Select(monthNumber => new SelectListItem { Value = monthNumber.ToString(), Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber) })
            .ToList();
        ViewBag.Years = (await mediator.Send(new GetWeatherYearsQuery()))
            .Years.Select(yearNumber => new SelectListItem { Value = yearNumber.ToString(), Text = yearNumber.ToString() })
            .ToList();

        return View();
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Upload()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}