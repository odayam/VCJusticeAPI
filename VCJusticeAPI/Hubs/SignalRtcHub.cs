using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
                userObj.connectionId = Context.ConnectionId;
                _context.Update(userObj);
                await _context.SaveChangesAsync();

                //var user = await _context.Users.FindAsync(newUser.Id);
                var participant = new Participant() { connectionId = Context.ConnectionId, user = userObj, isCurrent = false };
                await Clients.Others.SendAsync("NewUserArrived", JsonSerializer.Serialize(participant));
                participant.isCurrent = true;
                var allUsers = await _context.Users.Where(u => u.username != username && !string.IsNullOrEmpty(u.connectionId)).Select(u => new Participant() { connectionId = u.connectionId, user = u, isCurrent = false }).ToListAsync();
                allUsers.Add(participant);
                await Clients.Client(participant.connectionId).SendAsync("Hello", JsonSerializer.Serialize(allUsers));
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

        public async Task Mute(string user)
        {
            await Clients.Client(user).SendAsync("Mute", Context.ConnectionId);
        }
        public async Task IAmMute(string user, bool isMute)
        {
            await Clients.Others.SendAsync("MuteUpdate", user, isMute);
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
