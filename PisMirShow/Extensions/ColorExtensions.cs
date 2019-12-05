using System.Collections.Generic;

namespace PisMirShow.Extensions
{
	public static class ColorExtensions
	{
		private static readonly int CountOfColorsInHex = 16777215 - 1000000;

		public static List<string> GetHexColors(int count = 16)
		{
			if (count == 0) count = 16;

			int step = CountOfColorsInHex / count ;

			var result = new List<string>();

			int laststep = 0;
			for (;laststep < CountOfColorsInHex; laststep+=step)
			{
				if (count == 1)
				{
					result.Add("#" + CountOfColorsInHex.ToString("X"));
				}
				else
				{
					result.Add("#" +laststep.ToString("X"));
				}
			}
			if (count != 1)
			{
				result.RemoveAt(0);
			}
			return result;
		}
	}
}
