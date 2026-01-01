using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APICOOKIESTESTING.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hub;
        public LoginController(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }
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
            int i = 0;
            Task.Run(async () =>
            {
                while (i < 5)
                {
                    i++;
                    var hh = _hub.Clients.User("monaem");
                    await _hub.Clients.All.SendAsync("ReceiveMessage", $"{i} - [Login] Current Token: {jwt}");
                    await _hub.Clients.Group("monaem").SendAsync("ReceiveMessage", $"{i} - [Login] Current Token: {jwt} -  for monaem");
                    await Task.Delay(2000);
                }
            });
            return Ok(new { message = "Logged in" });
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new
            {
                name = "Monaem Khan",
                role = "Admin"
            });
        }

        [HttpGet("logout")]
        public IActionResult logout()
        {
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
