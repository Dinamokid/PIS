using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Models;

namespace PisMirShow.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(PisDbContext dbContext, IHostingEnvironment env, IToastNotification toastNotification) : base(dbContext, env, toastNotification)
        {
        }

        public IActionResult Index()
        {
            ViewBag.Messages = DbContext.Posts.AsNoTracking().OrderBy(u => u.Id);
            var user = GetCurrentUser();
            return View(user);
        }

        public IActionResult Profile()
        {
	        var user = GetCurrentUser();
            return View(user);
        }

        public IActionResult Dialogs()
        {
	        var user = GetCurrentUser();
	        return View(user);
        }

		public IActionResult About()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
        }

        public IActionResult DeleteMessages()
        {
            foreach (var temp in DbContext.Posts)
            {
                DbContext.Entry(temp).State = EntityState.Deleted;
            }

            DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateUser(User model)
        {
            var temp = DbContext.Users.FirstOrDefault(e => e.Id == model.Id);
            if (temp == null)
                return BadRequest();
            temp.LastName = model.LastName;
            temp.FirstName = model.FirstName;
            temp.OfficePost = model.OfficePost;
            temp.Phone = model.Phone;
            temp.Department = model.Department;
            temp.Email = model.Email;
            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public JsonResult GetCurrentUserByIdJson(int id)
        {
			var user = GetCurrentUser();
			if (user == null) return Json(new { message = "error" });
			return Json(new
			{
				user.Id,
				user.FirstName,
				user.LastName,
				user.OfficePost,
				user.Department,
				user.Phone,
				user.Email
			});
		}
    }
}
