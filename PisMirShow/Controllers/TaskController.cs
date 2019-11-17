using System;
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
    public class TaskController : BaseController
    {
        public TaskController(
	        PisDbContext dbContext, 
	        IHostingEnvironment env, 
	        IToastNotification toastNotification) : base(dbContext,env,toastNotification)
        {
        }

        public IActionResult AllTasks()
        {
            DellEmptyTasks();
            var task = DbContext.Tasks.Include(t => t.ToUser).Include(t => t.FromUser).AsNoTracking().ToList();
            return View(task);
        }

        public IActionResult Task(int id)
        {
            var task = DbContext.Tasks.Include(t => t.Files).Include(u=>u.FromUser).Include(u => u.ToUser).Include(t => t.Comments).ThenInclude(t => t.User).AsNoTracking().FirstOrDefault(t => t.Id == id);
            return View(task);
        }

        public IActionResult AddTask()
        {
            DbContext.Tasks.Add(new TaskItem());
            DbContext.SaveChanges();
            var model = DbContext.Tasks.Last();

            ViewBag.Users = DbContext.Users.AsNoTracking();

            return View(model);
        }

        [HttpPost]
        public IActionResult AddTask(TaskItem temp)
        {
            var task = DbContext.Tasks.First(t=>t.Id == temp.Id);
            task.DeadLine = temp.DeadLine;
            task.Files = temp.Files;
            task.FromUserId = temp.FromUserId;
            task.StartDate = temp.StartDate;
            task.Text = temp.Text;
            task.ToUserId = temp.ToUserId;
            task.EndDate = temp.EndDate;
            task.Status = temp.Status;
            task.Title = temp.Title;
            task.FilesId = temp.FilesId;
            DbContext.SaveChanges();
            return RedirectToAction("AllTasks");
        }

        public IActionResult EditTask(int id)
        {
            var model = DbContext.Tasks.FirstOrDefault(t => t.Id == id);

            if (model == null) RedirectToAction("AllTasks");

            ViewBag.Users = DbContext.Users.AsNoTracking();

            return View(model);
        }

        [HttpPost]
        public IActionResult EditTask(TaskItem temp)
        {
            var task = DbContext.Tasks.First(t=>t.Id == temp.Id);
            task.DeadLine = temp.DeadLine;
            task.Files = temp.Files;
            task.FromUserId = temp.FromUserId;
            task.StartDate = temp.StartDate;
            task.Text = temp.Text;
            task.ToUserId = temp.ToUserId;
            task.EndDate = temp.EndDate;
            task.Status = temp.Status;
            task.Title = temp.Title;
            task.FilesId = temp.FilesId;
            DbContext.SaveChanges();
            return RedirectToAction("AllTasks");
        }

        private void DellEmptyTasks()
        {
            var emptyTasks = DbContext.Tasks.Where(t => t.Title == null);
            DbContext.Tasks.RemoveRange(emptyTasks);
            DbContext.SaveChanges();
        }

        [Authorize(Roles = "Admin")]
		public void ClearComments()
        {
            DbContext.Posts.RemoveRange(DbContext.Posts);
            DbContext.SaveChanges();
        }

        public IActionResult AddCommentInTask(string text, int taskId, int userId)
        {
            var task = DbContext.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == taskId);
            if (task == null) return BadRequest();

            DbContext.TaskComments.Add(new TaskComments
            {
                TaskId = taskId,
                Text = text,
                CreateDate = DateTime.UtcNow,
                UserId = userId
            });

            DbContext.SaveChanges();
            return Ok();
        }

        [HttpPost]
        public IActionResult SetTaskStatus(int status, int taskId)
        {
            var temp = DbContext.Tasks.FirstOrDefault(f => f.Id == taskId);
            if (temp == null) return BadRequest();
            temp.Status = (TaskItem.TaskStatus)status;
            DbContext.SaveChanges();
            return Ok();
        }
    }
}
