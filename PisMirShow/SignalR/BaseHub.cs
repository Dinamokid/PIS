using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PisMirShow.SignalR
{
    [Authorize]
    public class BaseHub : Hub
    {
        protected readonly PisDbContext DbContext;
        private static readonly ConnectionMapping<string> Connections = new ConnectionMapping<string>();

        public BaseHub(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }
        
        public override async Task OnConnectedAsync()
        {
            var name = Context.User.Identity.Name;
            Connections.Add(name, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
			var name = Context.User.Identity.Name;
			Connections.Remove(name, Context.ConnectionId);
			await base.OnDisconnectedAsync(ex);
		}
    }
}