using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PisMirShow.ViewModels
{
	public class StatisticsViewModel
	{
		public double NotStarted { get; set; }
		public double Active { get; set; }
		public double Verification { get; set; }
		public double Confirmed { get; set; }
		public double Finished { get; set; }
	}
}
