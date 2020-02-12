using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PisMirShow.SignalR
{
	public class Dialogs : BaseHub
	{
        public Dialogs(PisDbContext dbContext) : base(dbContext)
        {
        }

        public async Task Send(string message, int dialogId)
        {
            var user = GetCurrentUser();

            if (!DbContext.UsersDialogs.Any(d => d.DialogId == dialogId && d.UserId == user.Id)) {
                await Clients.Caller.SendAsync("Error", "Не ломай меня");
            }

			DbContext.Messages.Add(new Message
			{
                AuthorId = user.Id,
                CreatedDate = DateTime.UtcNow,
                DialogId = dialogId,
                isReaded = false,
                Text = message,
            });

            var dialog = DbContext.Dialogs.Include(t => t.Users).First(t=>t.Id == dialogId);
            dialog.LastUpdate = DateTime.UtcNow;

			DbContext.SaveChanges();

            var dialogUsers = dialog.Users.Select(t => t.UserId).ToList();

            await Clients.Clients(Connections.GetConnectionsStrings(dialogUsers))
                .SendAsync("Send", message, user.GetFullName(), DateTime.UtcNow.ToString("g"), dialogId, user.Avatar);
        }
    }
}
