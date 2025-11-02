using WebAPI.Interface;

namespace WebAPI.Repository
{
    public class Test2 : ITest2
    {
        public readonly ISessionService _sessionService;
        public Test2(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public string GetTestMessage()
        {
            return "Hello from Test-2 Class and " + _sessionService.GetSessionMessage();
        }
    }
}
