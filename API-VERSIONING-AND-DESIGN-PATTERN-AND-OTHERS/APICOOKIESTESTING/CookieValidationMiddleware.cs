namespace APICOOKIESTESTING
{
    public class CookieValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if a specific cookie exists
            if (!context.Request.Cookies.ContainsKey("AuthCookie"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Missing cookie");
                return;
            }

            var cookieValue = context.Request.Cookies["AuthCookie"];
            if (cookieValue != "ValidToken123") // replace with your validation logic
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: Invalid cookie");
                return;
            }

            // Proceed if cookie is valid
            await _next(context);
        }
    }

    // Extension method for easier registration
    public static class CookieValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCookieValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieValidationMiddleware>();
        }
    }
}
