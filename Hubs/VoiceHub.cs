using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace like_icq.Hubs
{
    public class VoiceHub : Hub
    {
        // Bağlı kullanıcı adı ve oda takibi
        private static readonly ConcurrentDictionary<string, string> Names = new(); // connId -> name
        private static readonly ConcurrentDictionary<string, string> Rooms = new(); // connId -> room

        public override async Task OnConnectedAsync()
        {
            // Varsayılan isim/oda (Register çağrıldığında güncellenecek)
            var id = Context.ConnectionId;
            Names[id] = $"User-{id[..5]}";
            Rooms[id] = "Lobby";
            await Groups.AddToGroupAsync(id, "Lobby");
            await BroadcastUsers();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var id = Context.ConnectionId;
            if (Rooms.TryRemove(id, out var prevRoom))
            {
                await Groups.RemoveFromGroupAsync(id, prevRoom);
            }
            Names.TryRemove(id, out _);
            await BroadcastUsers();
            await base.OnDisconnectedAsync(exception);
        }

        // İsim kaydı (login sonrası)
        public async Task Register(string name)
        {
            Names[Context.ConnectionId] = string.IsNullOrWhiteSpace(name) ? Names[Context.ConnectionId] : name.Trim();
            await BroadcastUsers();
        }

        // Odaya katıl
        public async Task JoinRoom(string room)
        {
            var id = Context.ConnectionId;
            if (Rooms.TryGetValue(id, out var oldRoom))
            {
                await Groups.RemoveFromGroupAsync(id, oldRoom);
            }
            Rooms[id] = room;
            await Groups.AddToGroupAsync(id, room);

            var who = Names.TryGetValue(id, out var nm) ? nm : "Biri";
            await Clients.Group(room).SendAsync("ReceiveMessage", "Sistem", $"{who} odaya katıldı.");
            await BroadcastUsers();
        }

        // Chat (odaya yayın)
        public async Task SendMessage(string room, string message)
        {
            var id = Context.ConnectionId;
            var who = Names.TryGetValue(id, out var nm) ? nm : "Kullanıcı";
            await Clients.Group(room).SendAsync("ReceiveMessage", who, message);
        }

        // WebRTC sinyalleşme
        public async Task SendOffer(string targetId, string offer)
        {
            await Clients.Client(targetId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        public async Task SendAnswer(string targetId, string answer)
        {
            await Clients.Client(targetId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        public async Task SendIceCandidate(string targetId, string candidate)
        {
            await Clients.Client(targetId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        // Kullanıcı listesini (id, name, room) olarak yayınla
        private Task BroadcastUsers()
        {
            var users = Names.Select(kvp =>
            {
                var id = kvp.Key;
                var name = kvp.Value;
                Rooms.TryGetValue(id, out var room);
                return new { id, name, room = room ?? "" };
            }).ToList();

            return Clients.All.SendAsync("UpdateUserList", users);
        }
    }
}
