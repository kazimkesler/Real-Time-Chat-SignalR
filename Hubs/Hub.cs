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
        public async Task SendClientList()
        {
            var nicks = ClientInfos.Select(x => x.Nick ?? "Not Registered").ToList();
            await Clients.All.ClientList(nicks);
        }
        public async void Register(string nick)
        {
            var user = ClientInfos.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (user != null && nick != string.Empty && !ClientInfos.Any(x => x.Nick == nick))
            {
                await Clients.Others.ReceiveMessage(nick + (user.Nick != null ? "(" + user.Nick + ")" : string.Empty), "joined.");
                user.Nick = nick;
            }
            await SendClientList();
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

            await SendClientList();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = ClientInfos.First(x => x.ConnectionId == Context.ConnectionId);
            ClientInfos.Remove(user);

            if (user?.Nick != null)
                await Clients.Others.ReceiveMessage(user.Nick, "left.");

            await SendClientList();
        }
    }
}
