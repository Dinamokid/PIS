using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PisMirShow.Models;

namespace PisMirShow.Controllers
{
    public class HomeController : Controller
    {
        protected readonly PisDbContext DbContext;
        public HomeController(PisDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewBag.Messages = DbContext.Posts.AsNoTracking().OrderByDescending(u => u.Id);
            return View();
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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
