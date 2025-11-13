namespace StratasicDesignPattern
{
    public interface IMakePayment
    {
        string Make(string paymentName, decimal amount);
    }
    public class MakePayment : IMakePayment
    {
        private readonly IEnumerable<IPayment> _payments;
        public MakePayment(IEnumerable<IPayment> payments)
        {
            _payments = payments;
        }

        public string Make(string paymentName, decimal amount)
        {
            var selectedPayment = _payments.FirstOrDefault(p => p.Name == paymentName);
            if (selectedPayment != null)
            {
                return selectedPayment.Pay(amount);
            }
            return "Payment method not found.";
        }
    }
}
