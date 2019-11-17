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
    public class FileController : BaseController
    {
        public FileController(
	        PisDbContext dbContext, 
	        IHostingEnvironment env, 
	        IToastNotification toastNotification) : base(dbContext,env,toastNotification)
        {
        }

        public IActionResult AllFiles()
        {
			var user = GetCurrentUser();
			var filesQuery = DbContext.Tasks.Include(t => t.Files).Where(t => t.ToUserId == user.Id || t.FromUserId == user.Id).Select(t => t.Files)
				.Where(f => f.Count > 0).ToList();
			var files = new List<FileItem>();
			foreach (var temp in filesQuery)
			{
				files.AddRange(temp);
			}
			return View(files);
		}

        [HttpPost]
        public IActionResult GetFileInfo(int id)
        {
            var file = DbContext.Files.FirstOrDefault(f => f.Id == id);
            if (file != null)
            {
				return Json(new
                {
					name = file.Name,
					id = file.Id,
					type = file.Type,
					confirmed = file.Confirmed,
					confirmedDateTime = file.ConfirmedUserId != null ? " " + file.ConfirmedDateTime?.ToString("d") : "",
					confirmedByUser = file.ConfirmedUserId != null ? GetUserById(file.ConfirmedUserId)?.GetFullName() : "Не подтвержден",
					createdUser = file.CreatedUser.GetFullName(),
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
			file.DocType = model.DocType;
			file.Confirmed = model.Confirmed;
			file.ConfirmedUserId = model.ConfirmedUserId;
			file.ConfirmedDateTime = model.ConfirmedDateTime;

			if (model.Confirmed)
			{
				file.ConfirmedUserId = GetCurrentUser().Id;
				file.ConfirmedDateTime = DateTime.UtcNow;
			}

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
                    DocType = DocumentType.NotDetermined,
					CreatedUserId = GetCurrentUser().Id
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

    }
}
