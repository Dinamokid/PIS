using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PisMirShow;
using PisMirShow.Models;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        protected readonly PisDbContext DbContext;
        public ChatHub(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task Send(string message, string userName)
        {
            DbContext.Posts.Add(new WallPost()
            {
                Author = userName,
                Message = message
            });

            DbContext.SaveChanges();

            await Clients.All.SendAsync("Send", message, userName);
        }
    }
}