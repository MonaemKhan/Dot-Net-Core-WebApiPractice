using WebAPI.Interface;

namespace WebAPI.Repository
{
    public class SessionService : ISessionService
    {
        public string GetSessionMessage()
        {
            return "Hello from session Class";
        }
    }
}
