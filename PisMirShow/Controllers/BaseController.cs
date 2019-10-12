using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
            ViewBag.User = DbContext.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            await next();
        }

        protected User GetCurrentUser() => DbContext.Users.FirstOrDefault(u => u.Login == User.Identity.Name);

        protected User GetUserById(int? id)
        {
            var user = DbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.Password = null;
                return user;
            }

            return null;
        }

        protected JsonResult GetUserByIdJson(int id)
        {
            var temp = DbContext.Users.AsNoTracking().FirstOrDefault(e => e.Id == id);
            if (temp == null)
                return Json(new { message = "error" });
            return Json(new
            {
                Id = temp.Id,
                FirstName = temp.FirstName,
                LastName = temp.LastName,
                OfficePost = temp.OfficePost,
                Department = temp.Department,
                Phone = temp.Phone,
                Email = temp.Email
            });
        }
    }
}