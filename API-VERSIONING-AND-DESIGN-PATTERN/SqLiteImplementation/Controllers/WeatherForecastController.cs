using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace SqLiteImplementation.Controllers
{
    public class Product
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
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

        [HttpGet("SQLiteConn")]
        public IActionResult GetSQLiteConnectionString()
        {
            string connectionString = "Data Source=product.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Create table if it doesn’t exist
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    Quantity INTEGER NOT NULL
                );
            ";
                tableCmd.ExecuteNonQuery();

                return Ok("Database and Products table created successfully!");
            }
        }

        [HttpPost("AddProduct")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            string connectionString = "Data Source=product.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"
                INSERT INTO Products (Name, Price, Quantity)
                VALUES ($name, $price, $quantity);
            ";
                insertCmd.Parameters.AddWithValue("$name", product.Name);
                insertCmd.Parameters.AddWithValue("$price", product.Price);
                insertCmd.Parameters.AddWithValue("$quantity", product.Quantity);
                int rowsAffected = insertCmd.ExecuteNonQuery();
                return Ok($"{rowsAffected} product(s) added successfully!");
            }
        }

        [HttpGet("GetProducts")]
        public IActionResult GetProducts()
        {
            string connectionString = "Data Source=product.db";
            var products = new List<Product>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT Name, Price, Quantity FROM Products;";
                using (var reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            Name = reader.GetString(0),
                            Price = reader.GetDouble(1),
                            Quantity = reader.GetInt32(2)
                        };
                        products.Add(product);
                    }
                }
            }
            return Ok(products);
        }
    }
}
