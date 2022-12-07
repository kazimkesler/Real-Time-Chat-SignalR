using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChat.Hubs
{
    public class ChattingHub : Hub<IClientFunctions>
    {
        static List<ClientInfo> ClientInfos = new();
        public async void Register(string nick)
        {
            var user = ClientInfos.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null && nick != string.Empty && !ClientInfos.Any(x => x.Nick == nick))
            {
                user.Nick = nick;
                var nicks = ClientInfos.Where(x => x.Nick != null).Select(x => new User { Nick = x.Nick }).ToList();
                await Clients.All.ClientList(nicks);
                await Clients.Others.ReceiveMessage(user.Nick, "katıldı.");
            }
        }
        public async void SendMessageAsync(string message)
        {
            var user = ClientInfos.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user?.Nick != null)
                await Clients.Others.ReceiveMessage(user.Nick, message);
        }
        public async void SendPrivateMessageAsync(string message, string nick)
        {

            var receiver = ClientInfos.FirstOrDefault(x => x.Nick == nick);
            var sender = ClientInfos.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (receiver != null && sender?.Nick != null && sender != receiver)
                await Clients.Client(receiver.ConnectionId).ReceivePrivateMessage(sender.Nick, message);
        }
        public override async Task OnConnectedAsync()
        {
            var user = new ClientInfo { ConnectionId = Context.ConnectionId };
            ClientInfos.Add(user);
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = ClientInfos.First(x => x.ConnectionId == Context.ConnectionId);
            ClientInfos.Remove(user);
            var nicks = ClientInfos.Where(x => x.Nick != null).Select(x => new User { Nick = x.Nick }).ToList();
            await Clients.All.ClientList(nicks);
            if (user?.Nick != null)
                await Clients.Others.ReceiveMessage(user.Nick, "ayrıldı.");

        }
    }
}
