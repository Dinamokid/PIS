using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;

namespace PisMirShow
{
    [Authorize]
    public class DialogHub : Hub
    {
        protected readonly PisDbContext DbContext;
        public DialogHub(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task Send(string message, string userName, string sendTo)
        {
            User user = await DbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.Id.ToString() == sendTo);

            //DbContext.Posts.Add(new WallPost()
            //{
            //    Author = userName,
            //    Message = message
            //});

            //DbContext.SaveChanges();
            //var a = Context.User.Claims.Any(u => u.Value == "2");
            //var b = Context.ConnectionId;
            //await Clients.User(sendTo).SendAsync("Send", message, userName);
            await Clients.All.SendAsync("Send", message, userName);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"{Context.User} вошел в чат");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.User} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
    }
}