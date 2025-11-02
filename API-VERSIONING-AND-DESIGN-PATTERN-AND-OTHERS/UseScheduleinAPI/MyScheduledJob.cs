using Coravel.Invocable;

namespace UseScheduleinAPI
{
    public class MyScheduledJob : IInvocable
    {
        public Task Invoke()
        {
            Console.WriteLine($"Job running at {DateTime.Now}");
            return Task.CompletedTask;
        }
    }
}
