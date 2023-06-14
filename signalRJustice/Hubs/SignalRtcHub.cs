using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using VCJusticeAPI.Models;

namespace VCJusticeAPI.Hubs
{
    public class SignalRtcHub: Hub
    {
        private readonly UserContext _context;
        public async Task NewUser(string username)
        {
            if (_context.Users != null)
            {
                var userObj = await _context.Users.FirstAsync(u => u.username == username);
                //var user = await _context.Users.FindAsync(newUser.Id);
                var userInfo = new Participant() { connectionId = Context.ConnectionId, user = userObj, isCurrent = false };
                await Clients.Others.SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));
                userInfo.isCurrent = true;
                await Clients.Client(userInfo.connectionId).SendAsync("Hello", JsonSerializer.Serialize(userInfo));
            }
        }

        public async Task HelloUser(string username, string user)
        {
            var userObj= await _context.Users.FirstAsync(u => u.username == user);
            var userInfo = new Participant() { connectionId = Context.ConnectionId, user =  userObj};
            await Clients.Client(user).SendAsync("UserSaidHello", JsonSerializer.Serialize(userInfo));
        }


        public async Task SendSignal(string signal, string user)
        {
            await Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
        }

        public async Task Mute(string signal, string user)
        {
            await Clients.Client(user).SendAsync("Mute", Context.ConnectionId, signal);
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            await Clients.All.SendAsync("UserDisconnect", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public SignalRtcHub(UserContext context)
        {
            _context = context;
        }
    }
}
