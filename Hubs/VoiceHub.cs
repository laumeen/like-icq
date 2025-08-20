using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace LikeICQ.Hubs
{
    public class VoiceHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new();

        public override async Task OnConnectedAsync()
        {
            string username = Context.ConnectionId.Substring(0, 5); // kısa id
            Users[Context.ConnectionId] = username;

            await Clients.All.SendAsync("UserConnected", username, Users.Values.ToList());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Users.TryRemove(Context.ConnectionId, out string? username))
            {
                await Clients.All.SendAsync("UserDisconnected", username, Users.Values.ToList());
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string room, string user, string message)
        {
            await Clients.Group(room).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinRoom(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await Clients.Group(room).SendAsync("ReceiveMessage", "Sistem", $"{Users[Context.ConnectionId]} odaya katıldı.");
        }

        public async Task SendVoice(string room, string user, byte[] audioData)
        {
            await Clients.OthersInGroup(room).SendAsync("ReceiveVoice", user, audioData);
        }
    }
}
