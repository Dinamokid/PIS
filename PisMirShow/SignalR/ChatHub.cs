using Microsoft.AspNetCore.SignalR;
using PisMirShow.Models;
using System;
using System.Threading.Tasks;

namespace PisMirShow.SignalR
{
	public class ChatHub : BaseHub
	{
        public ChatHub(PisDbContext dbContext) : base(dbContext)
        {
        }

        public async Task Send(string message)
        {
            //TODO: рассылка конкретным юзерам
            //await Clients.Clients(Connections.GetConnectionsStrings(name)).SendAsync("Send", "TEST", name, DateTime.UtcNow.ToString("g"));
            //TODO: добавить задачи (открытие закрытие)
            //TODO: загрузка файлов (модель (id,ссылка, List<Users> кто утвердил))
            //TODO: прочитал в живую ленту
            var user = GetCurrentUser();

            DbContext.Posts.Add(new WallPost()
            {
                AuthorId = user.Id,
                Message = message,
                CreatedDate = DateTime.UtcNow
            });

            DbContext.SaveChanges();
            await Clients.All.SendAsync("Send", message, user.GetFullName(), DateTime.UtcNow.ToString("g"));
        }

    }
}
