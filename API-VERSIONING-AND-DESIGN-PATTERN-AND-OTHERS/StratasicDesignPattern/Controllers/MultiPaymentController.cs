using Microsoft.AspNetCore.Mvc;

namespace StratasicDesignPattern.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MultiPaymentController : ControllerBase
    {        

        private readonly ILogger<MultiPaymentController> _logger;
        private readonly IMakePayment _makePayment;

        public MultiPaymentController(ILogger<MultiPaymentController> logger, 
            IMakePayment makePayment)
        {
            _logger = logger;
            _makePayment = makePayment;
        }
        [HttpGet("MakePayment")]
        public string MakePayment(int amount = 500, string name = "Bkash")
        {
            var deviceId = Request.Cookies["DeviceId"];
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = Guid.NewGuid().ToString();

                // Set cookie, expires in 1 year
                Response.Cookies.Append("DeviceId", deviceId, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }
            _logger.LogInformation("Making payment using {PaymentMethod} for amount {Amount}", name, amount);
            return _makePayment.Make(name, amount);
        }
    }
}
