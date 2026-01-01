namespace APICOOKIESTESTING
{
    public class CookiesMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public CookiesMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        
        {
            var token = context.Request.Cookies["access_token"];
            if(context.Request.Path == "/api/Login/login")
            {
                
            }
            else if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Unauthorized Access");
                return;
            }
            await _requestDelegate(context);
        }
    }
}
