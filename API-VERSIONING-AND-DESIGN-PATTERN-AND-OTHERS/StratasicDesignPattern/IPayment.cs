namespace StratasicDesignPattern
{
    public interface IPayment
    {
        string Name { get; }
        public string Pay(decimal amount);
    }
}
