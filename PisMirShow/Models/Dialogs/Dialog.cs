using PisMirShow.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models.Dialogs
{
	public class Dialog
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public virtual List<UserDialog> Users { get; set; }

		public string Name { get; set; }

		public virtual List<Message> Messages { get; set; }

		public DateTime LastUpdate { get; set; }

		public EntryStatus EntryStatus { get; set; }

		public DialogType DialogType { get; set; }
	}

	public enum DialogType{
		dialog,
		groupchat
	};
}
