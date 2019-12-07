using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using PisMirShow.Extensions;
using PisMirShow.Models;
using PisMirShow.ViewModels;

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

	        var managerTasks = DbContext.Tasks.Where(t => t.FromUserId == user.Id);
	        var workerTasks = DbContext.Tasks.Where(t => t.ToUserId == user.Id);

	        double allManagerTasks = managerTasks.Count() / 100.0;
	        double allWorkerTasks = workerTasks.Count() / 100.0;
	        ViewBag.Statistic = new List<StatisticsViewModel>
	        {
		        new StatisticsViewModel
		        {
			        Active = Math.Round(managerTasks.Count(t => t.Status == TaskItem.TaskStatus.Active) / allManagerTasks, 3).ToString().Replace(',','.'),
			        Confirmed = Math.Round(managerTasks.Count(t => t.Status == TaskItem.TaskStatus.Confirmed) / allManagerTasks, 3).ToString().Replace(',','.'),
			        Finished = Math.Round(managerTasks.Count(t => t.Status == TaskItem.TaskStatus.Finished) / allManagerTasks, 3).ToString().Replace(',','.'),
			        NotStarted = Math.Round(managerTasks.Count(t => t.Status == TaskItem.TaskStatus.NotStarted) / allManagerTasks, 3).ToString().Replace(',','.'),
			        Verification = Math.Round(managerTasks.Count(t => t.Status == TaskItem.TaskStatus.Verification) / allManagerTasks, 3).ToString().Replace(',','.')
                },
		        new StatisticsViewModel
		        {
			        Active = Math.Round(workerTasks.Count(t => t.Status == TaskItem.TaskStatus.Active) / allWorkerTasks, 3).ToString().Replace(',','.'),
                    Confirmed = Math.Round(workerTasks.Count(t => t.Status == TaskItem.TaskStatus.Confirmed) / allWorkerTasks, 3).ToString().Replace(',','.'),
                    Finished = Math.Round(workerTasks.Count(t => t.Status == TaskItem.TaskStatus.Finished) / allWorkerTasks, 3).ToString().Replace(',','.'),
                    NotStarted = Math.Round(workerTasks.Count(t => t.Status == TaskItem.TaskStatus.NotStarted) / allWorkerTasks, 3).ToString().Replace(',','.'),
                    Verification = Math.Round(workerTasks.Count(t => t.Status == TaskItem.TaskStatus.Verification) / allWorkerTasks, 3).ToString().Replace(',','.'),
                }
	        };

	        var finishedTask = workerTasks.Where(t => t.EndDate != null).ToList();

	        var statisticWeek = new List<StatisticsDateViewModel>();
	        for (int i = 7; i > 0; i--)
	        {
				statisticWeek.Add( new StatisticsDateViewModel{
					Date = DateTime.UtcNow.TakeAwayDay(i - 1).ToLocalTime().Date.ToShortDateString(),
					Value = finishedTask.Count(t =>
						t.EndDate?.ToLocalTime() >= DateTime.UtcNow.TakeAwayDay(i - 1).ToLocalTime().Date
						&& t.EndDate?.ToLocalTime() <= DateTime.UtcNow.TakeAwayDay(i - 2).ToLocalTime().Date)
					}
				);
	        }
	        ViewBag.StatisticWeek = statisticWeek;

	        var statisticMouth = new List<StatisticsDateViewModel>();
	        for (int i = 30; i > 0; i--)
	        {
		        statisticMouth.Add( new StatisticsDateViewModel{
				        Date = DateTime.UtcNow.TakeAwayDay(i - 1).ToLocalTime().Date.ToShortDateString(),
				        Value = finishedTask.Count(t =>
					        t.EndDate?.ToLocalTime() >= DateTime.UtcNow.TakeAwayDay(i - 1).ToLocalTime().Date
					        && t.EndDate?.ToLocalTime() <= DateTime.UtcNow.TakeAwayDay(i - 2).ToLocalTime().Date)
			        }
		        );
	        }
	        ViewBag.StatisticMouth = statisticMouth;

	        var topTask = DbContext.Tasks.Include(t => t.ToUser).AsNoTracking().Where(t => t.Status == TaskItem.TaskStatus.Finished);

	        var usersInTasks = topTask.Where(t => t.EndDate.IsLessThanWeek())
		        .Select(t => t.ToUserId).Distinct().Select(temp => new StatisticsDateViewModel
	        {
		        Date = topTask.First(t => t.ToUserId == temp).ToUser.GetFullName(),
		        Value = topTask.Count(t => t.ToUserId == temp)
	        }).OrderByDescending(t=>t.Value).ToList();
	        ViewBag.TopTaskChartData = usersInTasks;
	        
	        ViewBag.ColorsList = ColorExtensions.GetHexColors(usersInTasks.Count);

            return View(user);
        }

        public IActionResult Dialogs()
        {
	        var user = GetCurrentUser();
	        return View(user);
        }

        public IActionResult Directory()
        {
	        var users = DbContext.Users.ToList();
			users.ForEach(t => t.Password = null);

			var model = new DirectoryDataViewModel
			{
				Users = users,
				DirectoryDataList = DbContext.DirectoryData.ToList()
			};

	        return View(model);
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

        [HttpPost]
        public IActionResult AddDirectoryData(DirectoryData model)
        {
	        DbContext.DirectoryData.Add(model);
	        DbContext.SaveChanges();
	        return RedirectToAction("Directory");
        }

        [HttpGet]
        public IActionResult DeleteDirectoryData(int id)
        {
	        var temp = DbContext.DirectoryData.FirstOrDefault(t => t.Id == id);
	        if (temp == null) ToastNotification.AddErrorToastMessage("Такого id не существует");
	        DbContext.DirectoryData.Remove(temp);
	        DbContext.SaveChanges();
	        return RedirectToAction("Directory");
        }
    }
}
