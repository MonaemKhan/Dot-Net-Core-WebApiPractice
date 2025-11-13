namespace StratasicDesignPattern
{
    public class BkashPayment : IPayment
    {
        public string Name => "Bkash";
        public string Pay(decimal amount)
        {
            return $"Paid from {Name}";
        }
    }

    public class RocketPayment : IPayment
    {
        public string Name => "Rocket";
        public string Pay(decimal amount)
        {
            return $"Paid from {Name}";
        }
    }

    public class NagadPayment : IPayment
    {
        public string Name => "Nagad";
        public string Pay(decimal amount)
        {
            return $"Paid from {Name}";
        }
    }

    public class CreditCardPayment : IPayment
    {
        public string Name => "Credit Card";
        public string Pay(decimal amount)
        {
            return $"Paid from {Name}";
        }
    }
}
