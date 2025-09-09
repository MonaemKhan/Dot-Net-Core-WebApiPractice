using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;

namespace CancleTokenPactice.Controllers
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

        [HttpGet("long-task")]
        public async Task<IActionResult> LongTask(CancellationToken cancellationToken)
        {
            // Simulate a long-running task
            for (int i = 0; i < 5; i++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // stop if client disconnected
                await Task.Delay(5000, cancellationToken);
                Console.WriteLine($"Iteration {i + 1} completed");
            }

            return Ok("Task finished successfully");
        }

        [HttpGet("Same-task")]
        public async Task<IActionResult> SameTask(CancellationToken cancellationToken,string name)
        {
            // Simulate a long-running task
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // stop if client disconnected
                    await Task.Delay(1000, cancellationToken);
                    Console.WriteLine($"Iteration {i + 1} completed for - " + name);
                }
                await NotifyClients("Task Finished for "+name,name);
            });

            return Ok("Task finished successfully");
        }

        private async Task NotifyClients(string message, string userId)
        {
            var bytes = Encoding.UTF8.GetBytes(message);

            foreach (var client in SataticClass.clients.Where(x=>x.UserId== userId).ToList()) // copy to avoid collection modified
            {
                if (client.socket.State == WebSocketState.Open)
                {
                    await client.socket.SendAsync(
                        new ArraySegment<byte>(bytes),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None);
                }
            }
        }

    }
}
