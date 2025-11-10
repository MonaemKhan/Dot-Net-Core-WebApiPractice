using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FindingIP_MacAddressOfClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
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

        [HttpGet("GetClientIP")]
        public string GetClientIP()
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            return remoteIpAddress != null ? remoteIpAddress.ToString() : "Unable to determine client IP address.";
        }

        [HttpGet("GetClientMacAddress")]
        public string GetClientMacAddress()
        {
            return "Unable to determine client MAC address over HTTP.";
        }

        private static string GetClientIPAddress()
        {
            string ip;
            ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            string n = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList.GetValue(0).ToString();

            if (!string.IsNullOrEmpty(ip))
            {
                string[] ipRange = ip.Split(',');
                string trueIP = ipRange[0].Trim();
                return trueIP;
            }
            else
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }


            return ip;
        }
    }
}
