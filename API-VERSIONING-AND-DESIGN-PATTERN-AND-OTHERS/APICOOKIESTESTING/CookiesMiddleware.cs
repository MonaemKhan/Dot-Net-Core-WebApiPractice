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

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Unauthorized Access");
                return;
            }
            // Proceed to the next middleware if the cookie is valid
            await _requestDelegate(context);
        }
    }
}
