using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PisMirShow.Models
{
	public class DirectoryData
	{
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public DirectoryType DirectoryType { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
	}
	public enum DirectoryType
	{
		[Display(Name = "Клиенты")]
		Clients,
	}
}
