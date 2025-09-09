using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace CancleTokenPactice.Controllers
{
    public class SataticClass
    {
        public static List<webSocketClass> clients = new List<webSocketClass>();
    }

    public class webSocketClass
    {
        public WebSocket socket { get; set; }
        public string UserId { get; set; }
    }
}
