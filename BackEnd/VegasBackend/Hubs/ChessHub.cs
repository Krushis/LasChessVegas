using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace VegasBackend.Hubs
{
    public class ChessHub : Hub
    {
        public async Task SendMove(object move)
        {
            await Clients.Others.SendAsync("ReceiveMove", move);
        }
    }
}
