using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Models;

namespace PisMirShow.Controllers
{
    public class BaseController : Controller
    {
        protected readonly PisDbContext DbContext;
        protected readonly IHostingEnvironment HostingEnv;
        protected readonly IToastNotification ToastNotification;

        public BaseController(PisDbContext dbContext, IHostingEnvironment env, IToastNotification toastNotification)
        {
			DbContext = dbContext;
            HostingEnv = env;
            ToastNotification = toastNotification;
		}

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
	        var user = GetCurrentUser();
	        if (user != null)
	        {
		        ViewBag.CurrentUserName = User.Identity.Name;
		        ViewBag.CurrentUserId = user.Id;
		        ViewBag.CurrentUserFullName = user.GetFullName();
	        }
	        await next();
        }

        protected User GetCurrentUser() => DbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Id.ToString() == User.Identity.Name);

        protected User GetUserById(int? id)
        {
	        return DbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        protected User GetUserByLogin(string login)
        {
	        return DbContext.Users.FirstOrDefault(u => u.Login == login);
        }
    }
}