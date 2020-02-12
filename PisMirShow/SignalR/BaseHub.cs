using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models.Account;

namespace PisMirShow.SignalR
{
    [Authorize]
    public class BaseHub : Hub
    {
        protected readonly PisDbContext DbContext;
        protected static readonly ConnectionMapping<int> Connections = new ConnectionMapping<int>();

        public BaseHub(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }
        
        public override async Task OnConnectedAsync()
        {
            Connections.Add(GetCurrentUserId(), Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
			var name = Context.User.Identity.Name;
			Connections.Remove(GetCurrentUserId(), Context.ConnectionId);
			await base.OnDisconnectedAsync(ex);
		}

        private int GetCurrentUserId(){
            if (int.TryParse(Context.User.Identity.Name, out int userId)) {
               return userId;
            } else {
                throw new Exception("Не удалось распарсить User ID");
            }
        }

        protected User GetCurrentUser() => DbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == GetCurrentUserId());
    }
}