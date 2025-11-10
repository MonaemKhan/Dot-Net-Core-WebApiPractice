namespace CLIENTIPMACADD
{
    public class ClientInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ConnectionInfo GetClientIp()
        {
            var context = _httpContextAccessor.HttpContext;
            var ip = _httpContextAccessor.HttpContext?.Connection;

            return ip;
        }
    }

}
