using PisMirShow.Models.Account;

namespace PisMirShow.Models.Dialogs
{
	public class UserDialog
	{
		public int UserId { get; set; }
		public virtual User User { get; set; }
		public int DialogId { get; set; }
		public virtual Dialog Dialog { get; set; }
	}
}
