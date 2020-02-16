using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
					Message = c.Messages.OrderByDescending(p => p.CreatedDate).FirstOrDefault()
				})
				.Where(m => m.Message != null)
				.Select(d => new DialogViewModel
				{
					DialogName = d.Dialog.Users.First(t => t.UserId != user.Id).User.GetFullName(),
					DialogPhotoUrl = d.Dialog.Users.First(t => t.UserId != user.Id).User.Avatar,
					DialogUserId = d.Dialog.Users.First(t => t.UserId != user.Id).User.Id,
					LastMessageAvatar = d.Message.Author.Avatar,
					LastMessageDate = d.Dialog.LastUpdate.ToString("g"),
					LastMessageText = d.Message.Text,
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
			userIds.Add(GetCurrentUser().Id);

			var dialogId = AddDialogAndUsers(userIds);

			return RedirectToAction($"Dialog", new { id = dialogId });
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

			if (!dialog.Users.Any(u => u.UserId == user.Id)) {
				return RedirectToAction("AllDialogs", "Dialog");
			}

			var messages = GetMessages(dialogId, offset: 0);

			ViewBag.Dialog = dialog;

			return View(messages);
		}

		public JsonResult GetMessagesJSON(int dialogId, int offset = 0) {
			return Json(GetMessages(dialogId, offset));
		}

		private List<Message> GetMessages(int dialogId, int offset) {
			return DbContext.Messages.Where(m => m.DialogId == dialogId)
				.Include(m => m.Author)
				.Include(m => m.Dialog)
				.AsNoTracking()
				.Skip(offset)
				.Take(20)
				.ToList();
		}

		private void AddUsersInDialog(List<int> usersId, int dialogId)
		{
			var dialog = DbContext.Dialogs.Include(t=>t.Users).FirstOrDefault(d => d.Id == dialogId);

			if (dialog != null)
			{
				foreach (var temp in usersId)
				{
					if (!dialog.Users.Any(t => t.UserId == temp)) {
						DbContext.UsersDialogs.Add(new UserDialog
						{
							DialogId = dialogId,
							UserId = temp
						});
					}
				}
				DbContext.SaveChanges();
			}
		}

		private int AddDialogAndUsers(List<int> usersId)
		{
			if (usersId.Count > 2) {
				var intersectionDialogs = DbContext.UsersDialogs
					.Where(t => t.Dialog.DialogType == DialogType.dialog && t.UserId == usersId[0] || t.UserId == usersId[1])
					.GroupBy(t => t.Dialog)
					.Select(group => new
					{
						Metric = group.Key,
						Count = group.Count()
					});

				if (!intersectionDialogs.Any(t => t.Count > 1))
				{
					RedirectToAction($"Dialog", new { id = intersectionDialogs.First(t => t.Count > 1).Metric.Id });
				}
			}

			var dialog = new Dialog
			{
				EntryStatus = Enums.EntryStatus.Readed,
				DialogType = usersId.Count > 2 ? DialogType.groupchat : DialogType.dialog,
				LastUpdate = DateTime.UtcNow
			};

			DbContext.Dialogs.Add(dialog);
			DbContext.SaveChanges();

			AddUsersInDialog(usersId, dialog.Id);

			return dialog.Id;
		}

		private List<User> GetUsersByDialog(int dialogId) {
			return DbContext.UsersDialogs.Where(t => t.DialogId == dialogId).Select(t => t.User).ToList();
		}
	}
}
