using Microsoft.AspNetCore.SignalR;
using PisMirShow.Models;
using System;
using System.Threading.Tasks;

namespace PisMirShow.SignalR
{
	public class Dialogs : BaseHub
	{
        public Dialogs(PisDbContext dbContext) : base(dbContext)
        {
        }
    }
}
