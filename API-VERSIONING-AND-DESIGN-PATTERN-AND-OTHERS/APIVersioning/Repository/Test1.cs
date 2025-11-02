using WebAPI.Interface;

namespace WebAPI.Repository
{
    public class Test1:ITest1
    {
        public readonly ISessionService _sessionService;
        public Test1(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public string GetTestMessage()
        {
            return "Hello from Test-1 Class and " + _sessionService.GetSessionMessage();
        }
    }
}
