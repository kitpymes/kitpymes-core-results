using System;
using System.Linq;
using Kitpymes.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Api.Result.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IApiResultOk), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IApiResultError), StatusCodes.Status400BadRequest)]
        public IApiResult GetOk() => AllSummaries;

        private static IApiResult AllSummaries
        {
            get
            {
                var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

                var rng = new Random();

                var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = new Random().Next(-20, 55),
                    Summary = summaries[rng.Next(summaries.Length)]
                });

                return ApiResult.Ok(x => x.WithData(list));
            }
        }
    }
}