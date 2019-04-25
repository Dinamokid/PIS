using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;

namespace PisMirShow.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(PisDbContext dbContext, IHostingEnvironment env) : base(dbContext, env)
        {
        }

        public IActionResult Index()
        {
            ViewBag.Messages = DbContext.Posts.AsNoTracking().OrderByDescending(u => u.Id);
            return View();
        }

        public IActionResult Dialogs()
        {
            return View();
        }

        public IActionResult Tasks()
        {
            ViewBag.Tasks = DbContext.Tasks.AsNoTracking();
            ViewBag.Users = DbContext.Users.AsNoTracking();
            return View();
        }

        [HttpPost]
        public IActionResult Tasks(TaskModel task)
        {

            DbContext.Tasks.Add(new TaskInSystem()
            {
                DeadLine = task.DeadLine,
                FromUser = task.FromUser,
                Status = TaskInSystem.TaskStatus.NotStarted,
                Text = task.Text,
                StartDate = DateTime.UtcNow,
                ToUser = task.ToUser,
            });
            //DbContext.SaveChanges();
            return RedirectToAction("Tasks");
        }

        [HttpPost]
        public IActionResult UploadFiles()
        {
            List<string> urlsList = new List<string>();
            if (Request.Form.Files != null)
            {
                var files = Request.Form.Files;
                
                foreach (var file in files)
                {
                    string filename = hostingEnv.WebRootPath + $@"\uploadedfiles\{file.FileName}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        var a = file.ContentType;
                        urlsList.Add($@"\uploadedfiles\{file.FileName}");
                        fs.Flush();
                    }
                }
            }
            return Json(urlsList);
        }

        public IActionResult AddTestData()
        {
            DbContext.Posts.Add(new WallPost()
            {
                Author = "text",
                Message = "message"
            });

            DbContext.SaveChanges();

            return RedirectToAction("Index");
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
