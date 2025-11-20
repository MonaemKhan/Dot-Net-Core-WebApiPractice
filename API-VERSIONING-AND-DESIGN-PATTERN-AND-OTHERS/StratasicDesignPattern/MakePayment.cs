namespace StratasicDesignPattern
{
    public interface IMakePayment
    {
        string Make(string paymentName, decimal amount);
    }
    public class MakePayment : IMakePayment
    {
        private readonly IEnumerable<IPayment> _payments;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MakePayment(IEnumerable<IPayment> payments,IHttpContextAccessor httpContextAccessor)
        {
            _payments = payments;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Make(string paymentName, decimal amount)
        {
            var data = _httpContextAccessor.HttpContext?.Request?.Cookies["DeviceId"];
            var selectedPayment = _payments.FirstOrDefault(p => p.Name == paymentName);
            if (selectedPayment != null)
            {
                return selectedPayment.Pay(amount);
            }
            return "Payment method not found.";
        }
    }
}
