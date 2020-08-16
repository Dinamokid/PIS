using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NETCore.Encrypt;
using NToastNotify;
using PisMirShow.Models;
using PisMirShow.ViewModels.Files;

namespace PisMirShow.Controllers
{
	[Authorize]
	public class FileController : BaseController
	{
		public FileController(
			PisDbContext dbContext,
			IHostingEnvironment env,
			IToastNotification toastNotification
			) : base(dbContext, env, toastNotification)
		{
		}

		public IActionResult AllFiles()
		{
			var result = AllAvailableTaskFiles();
			return View(result);
		}

		[HttpGet]
		public JsonResult AllAvailableTaskFilesJson()
		{
			var data = AllAvailableTaskFiles();

			return Json(data);
		}

		private List<FilesViewModel> AllAvailableTaskFiles()
		{
			var user = GetCurrentUser();
			var result = DbContext.Tasks.Include(t => t.Files).Where(t => t.ToUserId == user.Id || t.FromUserId == user.Id).Where(t => t.Files.Count > 0)
				.Select(t => new FilesViewModel
				{
					FileList = t.Files.Select(t => new FileItem
					{
						Confirmed = t.Confirmed,
						ConfirmedDateTime = t.ConfirmedDateTime,
						ConfirmedUserId = t.ConfirmedUserId,
						CreatedUserId = t.CreatedUserId,
						DocType = t.DocType,
						Id = t.Id,
						Name = t.Name,
						TaskId = t.TaskId,
						Type = t.Type
					}).ToList(),
					TaskName = t.Title,
					TaskId = t.Id
				}).ToList();

			foreach (var temp in result)
			{
				foreach (var item in temp.FileList)
				{
					item.Task = null;
					item.CreatedUser = null;
				}
			}

			return result;
		}

		[HttpGet]
		public JsonResult GetAvailableTasksJson()
		{
			var taskItemViewModels = AllAvailableTaskFiles().Select(t => new TaskItemViewModel
			{
				TaskId = t.TaskId,
				TaskName = t.TaskName
			});

			return Json(taskItemViewModels);
		}

		[HttpGet]
		public JsonResult GetAvailableExtensionsJson()
		{
			return Json(GetAllExtensions());
		}

		[HttpPost]
		public IActionResult GetFileInfo(int id)
		{
			var file = DbContext.Files.AsNoTracking().Include(t=>t.CreatedUser).FirstOrDefault(f => f.Id == id);
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

				fileData = EncryptProvider.AESEncrypt(fileData, "$eJbKuK1j43su0sFNGE*LxvmfBmPVtaF", "uhy7I!OECjWaV5nS");

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

				nameList.Add(new { file.Id, file.Name });
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
			var temp = DbContext.Files.FirstOrDefault(f => f.Id == id);
			if (temp != null)
			{
				byte[] mas = EncryptProvider.AESDecrypt(temp.File, "$eJbKuK1j43su0sFNGE*LxvmfBmPVtaF", "uhy7I!OECjWaV5nS");
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

		[HttpGet]
		public JsonResult GetFilesByExtensionJson(string extension)
		{
			if (extension == null)
			{
				return Json(null);
			}

			return Json(GetFilesByExtension(extension));
		}

		public List<FilesViewModel> GetFilesByExtension(string extension)
		{
			var taskFiles = AllAvailableTaskFiles();

			var result = taskFiles.Select(t => new FilesViewModel
			{
				FileList = t.FileList.Where(f => f.Name.Split('.')[f.Name.Split('.').Length - 1] == extension).ToList(),
				TaskId = t.TaskId,
				TaskName = t.TaskName
			}).Where(t => t.FileList.Count != 0).ToList();

			return result;
		}

		[HttpGet]
		public JsonResult GetFilesByTaskJson(string task)
		{
			if (task == null)
			{
				return Json(null);
			}

			return Json(GetFilesByTask(task));
		}

		public List<FilesViewModel> GetFilesByTask(string task)
		{
			var taskFiles = AllAvailableTaskFiles();

			var result = taskFiles.Where(t => t.TaskId == int.Parse(task)).Select(t => new FilesViewModel
			{
				FileList = t.FileList.ToList(),
				TaskId = t.TaskId,
				TaskName = t.TaskName
			}).Where(t => t.FileList.Count != 0).ToList();

			return result;
		}

		[HttpGet]
		public JsonResult GetFilesByNameJson(string name)
		{
			if (name == null)
			{
				return Json(null);
			}

			return Json(GetFilesByName(name));
		}

		private List<FilesViewModel> GetFilesByName(string name)
		{
			var taskFiles = AllAvailableTaskFiles();

			var result = taskFiles.Select(t => new FilesViewModel
			{
				FileList = t.FileList.Where(f => f.Name.Contains(name)).ToList(),
				TaskId = t.TaskId,
				TaskName = t.TaskName
			}).Where(t => t.FileList.Count != 0).ToList();

			return result;
		}

		[HttpPost]
		private List<string> GetAllExtensions()
		{
			var result = new List<string>();
			var fileNames = DbContext.Files.Select(f => f.Name);
			foreach (var temp in fileNames)
			{
				result.Add(temp.Split('.')[temp.Split('.').Length - 1]);
			}

			return result.Distinct().ToList();
		}
	}
}
