using System.Collections.Generic;
using PisMirShow.Models;
using PisMirShow.Models.Account;

namespace PisMirShow.ViewModels
{
	public class DirectoryDataViewModel
	{
		public List<User> Users { get; set; } 
		public List<DirectoryData> DirectoryDataList { get; set; } 
	}
}
