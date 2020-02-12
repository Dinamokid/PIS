using PisMirShow.Enums;
using System;

namespace PisMirShow.ViewModels.Dialogs
{
	public class DialogViewModel
	{
		public string DialogPhotoUrl { get; set; }
		public string DialogName { get; set; }
		public string LastMessageAvatar { get; set; }
		public string LastMessageText { get; set; }
		public string LastMessageDate { get; set; }
		public string CurrentUserAvatar { get; set; }
		public EntryStatus EntryStatus { get; set; }
		public int DialogId { get; set; }
		public int DialogUserId { get; set; }
	}
}
