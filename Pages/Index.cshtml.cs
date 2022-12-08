using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RealTimeChat.Hubs;

namespace RealTimeChat.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHubContext<ChattingHub, IClientFunctions> _hubContext;
        public IndexModel(ILogger<IndexModel> logger, IHubContext<ChattingHub, IClientFunctions> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public void OnGet()
        {
            _hubContext.Clients.All.ReceiveMessage("Server", "an user has joined.");
        }
    }
}
