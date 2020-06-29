using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NToastNotify;
using PisMirShow.Models.Account;
using PisMirShow.Models.Dialogs;
using PisMirShow.ViewModels.Account;
using PisMirShow.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PisMirShow.Controllers
{
	[Authorize]
	public class DialogController : BaseController
	{
		public DialogController(PisDbContext dbContext, IHostingEnvironment env, IToastNotification toastNotification) : base(dbContext, env, toastNotification)
		{
		}

		[Route("Dialog/Index")]
		[Route("Dialog/AllDialogs")]
		[Route("Dialogs")]
		[Route("Dialog/Dialogs")]
		public IActionResult AllDialogs()
		{
			ViewBag.Dialogs = GetDialogs(offset: 0);
			var user = GetCurrentUser().GetUserSafe();
		
			return View(user);
		}

		public JsonResult GetDialogsJSON(int offset = 0)
		{
			return Json(GetDialogs(offset));
		}

		private List<DialogViewModel> GetDialogs(int offset)
		{
			var user = GetCurrentUser().GetUserSafe();
			return DbContext.Dialogs.AsNoTracking()
				.Where(d => d.Users.Any(t => t.UserId == user.Id))
				.Include(d => d.Users)
					.ThenInclude(d => d.User)
				.Include(d => d.Messages)
					.ThenInclude(t => t.Author)
				.OrderByDescending(d => d.LastUpdate)
				.Select(c => new
				{
					Dialog = c,
					Message = c.Messages.OrderByDescending(p => p.CreatedDate).FirstOrDefault(),
					RecipientUser = c.Users.FirstOrDefault(t => t.UserId != user.Id) ?? c.Users.First(),
				})
				.ToList()
				.Select(d => new DialogViewModel
				{
					DialogName = d.Dialog.Name ?? d.RecipientUser.User.GetFullName(),
					DialogPhotoUrl = d.Dialog.Avatar ?? d.RecipientUser.User.Avatar,
					DialogUserId = d.Dialog.Name == null ? d.RecipientUser.User.Id : -1,
					LastMessageAvatar = d.Message?.Author?.Avatar,
					LastMessageDate = d.Dialog.LastUpdate.ToString("g"),
					LastMessageText = d.Message?.Text,
					EntryStatus = d.Dialog.EntryStatus,
					CurrentUserAvatar = user.Avatar,
					DialogId = d.Dialog.Id,
				})
				.Skip(offset)
				.Take(20)
				.ToList();
		}

		public IActionResult AddDialog()
		{
			var users = DbContext.Users.Select(u => new UserViewModel
			{
				Id = u.Id,
				FullName = u.GetFullName(),
				Avatar = u.Avatar
			}).ToList();
			return PartialView("_AddDialog", users);
		}

		[HttpPost]
		public IActionResult AddDialog(List<int> userIds)
		{
			if (!userIds.Any())
			{
				return RedirectToAction("AllDialogs");
			}

			var user = GetCurrentUser();
			if (!userIds.Contains(user.Id))
			{
				userIds.Add(user.Id);
			}
				
			var dialogId = AddDialogAndUsers(userIds);

			return RedirectToAction("Dialog", new { dialogId = dialogId });
		}

		[Route("Dialog/WithId/{dialogId}")]
		public IActionResult Dialog(int dialogId)
		{
			var dialog = DbContext.Dialogs.AsNoTracking()
						.Include(t => t.Users)
						.ThenInclude(d => d.User)
						.FirstOrDefault(d => d.Id == dialogId);

			if (dialog == null)
			{
				return RedirectToAction("AllDialogs", "Dialog");
			}

			var user = GetCurrentUser().GetUserSafe();

			if (!dialog.Users.Any(u => u.UserId == user.Id))
			{
				return RedirectToAction("AllDialogs", "Dialog");
			}

			var messages = GetMessages(dialogId, offset: 0);

			ViewBag.Dialog = dialog;

			return View(messages);
		}

		[HttpGet]
		public JsonResult GetMessagesJSON(int dialogId, int offset = 0)
		{
			var messages = GetMessages(dialogId, offset);
			var orderedMessages = messages.MessageList.OrderByDescending(t => t.CreatedDate);

			return Json(
			new {
				Messages = orderedMessages.Select(t => new {
					FullName = t.Author.GetFullName(),
					Message = t.Text,
					Date = t.CreatedDate.ToString("g"),
				}),
				TotalCount = messages.TotalCount
			}
			);
		}

		private Messages GetMessages(int dialogId, int offset)
		{
			var messages = DbContext.Messages.Where(m => m.DialogId == dialogId)
				.Include(m => m.Author)
				.Include(m => m.Dialog)
				.AsNoTracking()
				.OrderByDescending(m => m.CreatedDate)
				.Skip(offset)
				.Take(50)
				.OrderBy(m => m.CreatedDate)
				.ToList();

			return new Messages
			{
				MessageList = messages,
				TotalCount = DbContext.Messages.Where(m => m.DialogId == dialogId).Count()
			};
		}

		private void AddUsersInDialog(List<int> usersId, int dialogId)
		{
			var dialog = DbContext.Dialogs.Include(t => t.Users).FirstOrDefault(d => d.Id == dialogId);

			if (dialog != null)
			{
				var users = DbContext.Users.AsNoTracking().Where(u => usersId.Any(t => t == u.Id)).ToList();
				foreach (var temp in users)
				{
					if (!dialog.Users.Any(t => t.UserId == temp.Id))
					{
						DbContext.UsersDialogs.Add(new UserDialog
						{
							DialogId = dialogId,
							UserId = temp.Id
						});
					}
				}

				if (users.Count() > 2){
					dialog.Name = string.Join(", ", users.Select(t => t.FirstName));
					dialog.Avatar = "/files/avatars/group.jpg";
				}

				DbContext.SaveChanges();
			}
		}

		private int AddDialogAndUsers(List<int> usersIds)
		{
			Dialog dialogIfExist = null;
			var dialogType = GetDialogType(usersIds.Count);

			if (dialogType == DialogType.monolog)
			{
				dialogIfExist = DbContext.UsersDialogs.Include(t => t.Dialog).AsNoTracking()
									.FirstOrDefault(t => t.Dialog.DialogType == DialogType.monolog && t.UserId == usersIds[0])?.Dialog;
			}
			else if (dialogType == DialogType.dialog)
			{
				dialogIfExist = DbContext.UsersDialogs.Include(t => t.Dialog).ToList()
									.Where(t => t.Dialog.DialogType == DialogType.dialog && t.UserId == usersIds[0] || t.UserId == usersIds[1])
									.GroupBy(t => t.Dialog)
									.Select(group => new
									{
										Metric = group.Key,
										Count = group.Count()
									}).FirstOrDefault(t => t.Count > 1)?.Metric;
			}

			if (dialogIfExist != null)
			{
				return dialogIfExist.Id;
			}

			var dialog = new Dialog
			{
				EntryStatus = Enums.EntryStatus.Readed,
				DialogType = dialogType,
				LastUpdate = DateTime.UtcNow
			};

			DbContext.Dialogs.Add(dialog);
			DbContext.SaveChanges();

			AddUsersInDialog(usersIds, dialog.Id);

			return dialog.Id;
		}

		private List<User> GetUsersByDialog(int dialogId)
		{
			return DbContext.UsersDialogs.Where(t => t.DialogId == dialogId).Select(t => t.User).ToList();
		}

		private DialogType GetDialogType(int count) {
			switch (count) { 
				case 1:
					return DialogType.monolog;
				case 2:
					return DialogType.dialog;
				default:
					return DialogType.groupchat;
			}
		}
	}
}
