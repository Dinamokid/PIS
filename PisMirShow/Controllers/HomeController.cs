﻿using System;
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

        public IActionResult AllTasks()
        {
            DellEmptyTasks();
            ViewBag.Tasks = DbContext.Tasks.AsNoTracking();
            return View();
        }

        public IActionResult Task(int id)
        {
            var task = DbContext.Tasks.Include(t => t.Files).Include(t => t.Comments).ThenInclude(t => t.User).AsNoTracking().FirstOrDefault(t => t.Id == id);
            ViewBag.Task = task;
            return View();
        }

        public IActionResult AddTask()
        {
            DbContext.Tasks.Add(new TaskInSystem());
            DbContext.SaveChanges();
            var model = DbContext.Tasks.Last();

            ViewBag.Users = DbContext.Users.AsNoTracking();

            return View(model);
        }

        [HttpPost]
        public IActionResult AddTask(TaskInSystem temp)
        {
            var task = DbContext.Tasks.First(t=>t.Id == temp.Id);
            task.DeadLine = temp.DeadLine;
            task.Files = temp.Files;
            task.FromUser = temp.FromUser;
            task.StartDate = temp.StartDate;
            task.Text = temp.Text;
            task.ToUser = temp.ToUser;
            task.EndDate = temp.EndDate;
            task.Status = temp.Status;
            task.Title = temp.Title;
            task.FilesId = temp.FilesId;
            DbContext.SaveChanges();
            return RedirectToAction("AllTasks");
        }

        public void DellEmptyTasks()
        {
            var emptyTasks = DbContext.Tasks.Where(t => t.Title == null);
            DbContext.Tasks.RemoveRange(emptyTasks);
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
                    string filename = hostingEnv.WebRootPath + $@"\uploadedfiles\{file.FileName}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        var a = file.ContentType;
                        nameList.Add(file.FileName);
                        fs.Flush();
                    }
                }
            }
            return Json(nameList);
        }

        [HttpPost]
        public JsonResult UploadFilesInBD(int taskId)
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

                FileInSystem file = new FileInSystem()
                {
                    File = fileData,
                    Type = temp.ContentType,
                    Сonfirmed = false,
                    TaskId = taskId,
                    Name = temp.FileName
                };

                DbContext.Files.Add(file);
                DbContext.SaveChanges();

                var last = DbContext.Files.AsNoTracking().Last();
                nameList.Add(new { Id = last.Id, Name = last.Name });
            }

            return Json(nameList);
        }

        public void DeleteFile(int id)
        {
            var file = DbContext.Files.FirstOrDefault(f => f.Id == id);
            DbContext.Files.Remove(file);
            DbContext.SaveChanges();
        }

        public FileResult GetFileById(int id)
        {
            var temp = DbContext.Files.FirstOrDefault(f=>f.Id == id);
            if (temp != null)
            {
                byte[] mas = temp.File;
                string file_type = temp.Type;
                string file_name = temp.Name;
                return File(mas, file_type, file_name);
            }
            return null;
        }

        public FileResult GetFilesByTask(int taskId)
        {
            var temp = DbContext.Files.FirstOrDefault(f => f.TaskId == taskId);
            if (temp != null)
            {
                byte[] mas = temp.File;
                string file_type = temp.Type;
                string file_name = temp.Name;
                return File(mas, file_type, file_name);
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

    }
}
