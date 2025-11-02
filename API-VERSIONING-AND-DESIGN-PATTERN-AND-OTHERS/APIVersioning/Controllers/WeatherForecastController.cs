using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebAPI.Interface;
using WebAPI.Model;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/WeatherForecast/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMainCLass _MainCLass;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,IMainCLass mainCLass)
        {
            _logger = logger;
            _MainCLass = mainCLass;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Test")]
        public IActionResult Test()
        {
            return Ok(_MainCLass.GetMainMessage().GetTestMessage());
        }

        [HttpGet("Test2")]
        public IActionResult Test2()
        {
            return Ok(_MainCLass.GetMainMessage2().GetTestMessage());
        }

        [HttpGet("Test3"),MapToApiVersion("1.0")]
        public IActionResult Test3V1()
        {
            List<object> obj = new List<object>();
            var data = new UserModelBuilder().withName("Monaem Khan")
                .withEmail("khanMonaem07@gmail.com")
                .withAge(28)
                .withPortfolio("Test Class 01");

            obj.Add(data.Build());
            return Ok(obj);
        }

        [HttpGet("Test3"),MapToApiVersion("2.0")]
        public IActionResult Test3V2()
        {
            List<object> obj = new List<object>();
            var data = new UserModelBuilder().withName("Monaem Khan")
                .withEmail("khanMonaem07@gmail.com")
                .withAge(28)
                .withPortfolio("Test Class 01");

            obj.Add(data.Build());
            obj.Add(data.Builder());
            return Ok(obj);
        }
    }
}
