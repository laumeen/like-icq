using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LikeICQ.Hubs
{
    public class VoiceHub : Hub
    {
        // Kullanıcı odaya katılır
        public async Task Join(string userName, string roomName)
        {
            // Kullanıcı SignalR grubuna ekleniyor
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            // Sadece o odadaki diğer kullanıcılara haber ver
            await Clients.Group(roomName).SendAsync("UserJoined", userName);
        }

        // Kullanıcı odadan ayrıldığında
        public async Task Leave(string userName, string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("UserLeft", userName);
        }

        // Basit test için mesaj fonksiyonu
        public async Task SendMessage(string roomName, string userName, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", userName, message);
        }
    }
}
