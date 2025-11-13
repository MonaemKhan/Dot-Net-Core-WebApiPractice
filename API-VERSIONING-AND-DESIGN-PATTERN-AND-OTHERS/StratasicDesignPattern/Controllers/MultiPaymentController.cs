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
        public string MakePayment(int amount, string name)
        {
            _logger.LogInformation("Making payment using {PaymentMethod} for amount {Amount}", name, amount);
            return _makePayment.Make(name, amount);
        }
    }
}
