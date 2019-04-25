using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PisMirShow.Controllers
{
    public class BaseController : Controller
    {
        protected readonly PisDbContext DbContext;
        protected IHostingEnvironment hostingEnv;

        public BaseController(PisDbContext dbContext, IHostingEnvironment env)
        {
            DbContext = dbContext;
            hostingEnv = env;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.User = DbContext.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            await next();
        }
    }
}