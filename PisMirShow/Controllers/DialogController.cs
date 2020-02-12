﻿using Microsoft.AspNetCore.Authorization;
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
			var user = GetCurrentUser().GetUserSafe();

			var dialogs = DbContext.Dialogs.AsNoTracking()
				.Where(d => d.Users.Any(t => t.UserId == user.Id))
				.Include(d => d.Users)
					.ThenInclude(d => d.User)
				.Include(d => d.Messages)
					.ThenInclude(t => t.Author)
				.Include(d => d.Messages)
					.ThenInclude(t => t.Recipient)
				.OrderByDescending(d => d.LastUpdate)
				.Select(c => new
				{
					Dialog = c,
					Message = c.Messages.OrderByDescending(p => p.CreatedDate).FirstOrDefault()
				})
				.Select(a => a.Dialog)
				.Select(d => new DialogViewModel
				{
					DialogName = d.Users.First(t => t.UserId != user.Id).User.GetFullName(),
					DialogPhotoUrl = d.Users.First(t => t.UserId != user.Id).User.Avatar,
					DialogUserId = d.Users.First(t => t.UserId != user.Id).User.Id,
					LastMessageAvatar = d.Messages.FirstOrDefault().Author.Avatar,
					LastMessageDate = d.LastUpdate.ToString("g"),
					LastMessageText = d.Messages.FirstOrDefault().Text,
					EntryStatus = d.EntryStatus,
					CurrentUserAvatar = user.Avatar,
					DialogId = d.Id,
				})
				.ToList();

			//TODO: Не отображать диалоги без сообщений.

			ViewBag.Dialogs = dialogs;

			return View(user);
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

		[Route("Dialog/WithId/{id}")]
		public string Dialog(int id)
		{
			return $"Диалог c ID:{id}";
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
