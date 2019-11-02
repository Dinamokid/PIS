using System;
using System.Collections.Generic;
using System.IO;
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
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Dialogs()
        {
            return View();
        }

        public IActionResult AllTasks()
        {
            DellEmptyTasks();
            var task = DbContext.Tasks.Include(t => t.ToUser).Include(t => t.FromUser).AsNoTracking().ToList();
            return View(task);
        }

        //[Authorize(Roles = "Admin")]
		public IActionResult About()
        {
            return View();
        }

        public IActionResult Documents()
        {
            return View();
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

        public IActionResult AllFiles()
        {
            var files = DbContext.Files.AsNoTracking().ToList();
            return View(files);
        }

        [HttpPost]
        public IActionResult GetFileInfo(int id)
        {
            var file = DbContext.Files.AsNoTracking().FirstOrDefault(f => f.Id == id);
            if (file != null)
            {
                return Json(new
                {
					name = file.Name,
					id = file.Id,
					type = file.Type,
					confirmed = file.Confirmed,
					confirmedDateTime = file.ConfirmedDateTime,
					confirmedByUser = GetUserById(file.ConfirmedUserId),
					createdUser = file.CreatedUser,
                    docType = (int)file.DocType
                });
            }
            return BadRequest();
        }

        public IActionResult SetFileInfo(FileItem model)
        {
            var file = DbContext.Files.FirstOrDefault(f => f.Id == model.Id);
            if (file == null) return BadRequest();
            file.Name = model.Name;
            if (model.Confirmed)
            {
	            file.Confirmed = model.Confirmed;
	            file.ConfirmedUserId = GetCurrentUser().Id;
			}
            file.DocType = model.DocType;
			DbContext.SaveChanges();
            return Ok();
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
        public JsonResult UploadFiles()
        {
            List<string> nameList = new List<string>();
            if (Request.Form.Files != null)
            {
                var files = Request.Form.Files;

                foreach (var file in files)
                {
                    string filename = HostingEnv.WebRootPath + $@"\uploadedfiles\{file.FileName}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        nameList.Add(file.FileName);
                        fs.Flush();
                    }
                }
            }
            return Json(nameList);
        }

        [HttpPost]
        public JsonResult UploadFilesInBd(int taskId)
        {
			var nameList = new List<Tuple<int, string>>()
                .Select(t => new { Id = t.Item1, Name = t.Item2 }).ToList();
            foreach (var temp in Request.Form.Files)
            {
                byte[] fileData;

                using (var binaryReader = new BinaryReader(temp.OpenReadStream()))
                {
                    fileData = binaryReader.ReadBytes((int)temp.Length);
                }

                FileItem file = new FileItem()
                {
                    File = fileData,
                    Type = temp.ContentType,
                    Confirmed = false,
                    TaskId = taskId,
                    Name = temp.FileName,
                    DocType = DocumentType.NotDetermined
                };

                DbContext.Files.Add(file);
                DbContext.SaveChanges();

                var last = DbContext.Files.AsNoTracking().Last();
                nameList.Add(new {last.Id, last.Name });
            }

            return Json(nameList);
        }

        [HttpPost]
        public IActionResult DeleteFile(int id)
        {
            var file = DbContext.Files.FirstOrDefault(f => f.Id == id);
            if (file != null)
            {
                DbContext.Files.Remove(file);
                DbContext.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        public FileResult GetFileById(int id)
        {
            var temp = DbContext.Files.FirstOrDefault(f=>f.Id == id);
            if (temp != null)
            {
                byte[] mas = temp.File;
                string fileType = temp.Type;
                string fileName = temp.Name;
                DocumentType TypeDoc = temp.DocType;
                return File(mas, fileType, fileName);
            }
            return null;
        }

        public FileResult GetFilesByTask(int taskId)
        {
            var temp = DbContext.Files.FirstOrDefault(f => f.TaskId == taskId);
            if (temp != null)
            {
                byte[] mas = temp.File;
                string fileType = temp.Type;
                string fileName = temp.Name;
                return File(mas, fileType, fileName);
            }
            return null;
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
        public IActionResult SetTaskStatus(int status, int taskId)
        {
            var temp = DbContext.Tasks.FirstOrDefault(f => f.Id == taskId);
            if (temp == null) return BadRequest();
            temp.Status = (TaskItem.TaskStatus)status;
            DbContext.SaveChanges();
            return Ok();
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
        public JsonResult GetUserByIdJson(int id)
        {
            return base.GetUserByIdJson(id);
        }
    }
}
