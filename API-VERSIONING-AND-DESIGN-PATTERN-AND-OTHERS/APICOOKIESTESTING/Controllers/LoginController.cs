using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APICOOKIESTESTING.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        int num = 1;
        [HttpPost("login")]
        public IActionResult Login()
        {
            // Normally you validate username/password here
            var jwt = "TokenNum."+num;
            num++;

            Response.Cookies.Append("access_token", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // HTTPS only
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Logged in" });
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var token = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            return Ok(new
            {
                name = "Monaem Khan",
                role = "Admin"
            });
        }

        [HttpGet("logout")]
        public IActionResult logout()
        {
            var token = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            Response.Cookies.Append("access_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,              // HTTPS only
                SameSite = SameSiteMode.None
            });

            return Ok(new
            {
                message = "User Logged Out"
            });
        }
    }
}
