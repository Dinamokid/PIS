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
    public class ChatHub : Hub
    {
        protected readonly PisDbContext DbContext;
        public ChatHub(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }
        public async Task Send(string message, string userName)
        {
            //TODO: https://metanit.com/sharp/aspnet5/18.3.php
            //TODO: рассылка конкретным юзерам
            //TODO: добавить задачи (открытие закрытие)
            //TODO: загрузка файлов (модель (id,ссылка, List<Users> кто утвердил))
            //TODO: прочитал в живую ленту

            DbContext.Posts.Add(new WallPost()
            {
                Author = userName,
                Message = message,
                CreatedDate = DateTime.UtcNow
            });

            DbContext.SaveChanges();
            var a = Context.User.Claims.Any(u => u.Value == "2");
            await Clients.All.SendAsync("Send", message, userName, DateTime.UtcNow.ToString("g"));
        }

        //public override async Task OnConnectedAsync()
        //{
        //    await Clients.All.SendAsync("Notify", $"{Context.User} вошел в чат");
        //    await base.OnConnectedAsync();
        //}
        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    await Clients.All.SendAsync("Notify", $"{Context.User} покинул в чат");
        //    await base.OnDisconnectedAsync(exception);
        //}
    }
}