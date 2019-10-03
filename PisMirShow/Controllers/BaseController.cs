using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            ViewBag.User = DbContext.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            await next();
        }

        protected User GetCurrentUser() => DbContext.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
    }
}