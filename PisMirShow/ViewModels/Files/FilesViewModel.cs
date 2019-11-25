using System.Collections.Generic;
using PisMirShow.Models;

namespace PisMirShow.ViewModels.Files
{
	public class FilesViewModel
	{
		public List<FileItem> FileList { get; set; }
		public string TaskName {get; set;}
		public int TaskId {get; set;}
	}
}
