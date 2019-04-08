using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PisMirShow.Models;

namespace PisMirShow
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
            //TODO: авторизация 
            //TODO: рассылка конкретным юзерам
            //TODO: добавить задачи (открытие закрытие)
            //TODO: загрузка файлов (модель (id,ссылка, List<Users> кто утвердил))
            //TODO: прочитал в живую ленту

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