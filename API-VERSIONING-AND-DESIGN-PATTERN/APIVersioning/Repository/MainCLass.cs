using WebAPI.Interface;

namespace WebAPI.Repository
{
    public class MainCLass : IMainCLass
    {
        private readonly ISessionService _sessionService;
        public MainCLass(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public ITest1 GetMainMessage()
        {
            return new Test1(_sessionService);
        }

        public ITest2 GetMainMessage2()
        {
            return new Test2(_sessionService);
        }
    }
}
