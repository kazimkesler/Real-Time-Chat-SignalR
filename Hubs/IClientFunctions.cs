using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTimeChat.Hubs
{
    public interface IClientFunctions
    {
        Task Register(string nick);
        Task ReceiveMessage(string nick, string message);
        Task ReceivePrivateMessage(string nick, string message);
        Task ClientList(List<User> clients);
    }
}
