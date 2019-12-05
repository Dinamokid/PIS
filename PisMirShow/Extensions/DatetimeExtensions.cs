using System;

namespace PisMirShow.Extensions
{
	public static class DatetimeExtensions
	{
		public static DateTime TakeAwayDay(this DateTime firstDateTime, int count)
		{
			return firstDateTime - TimeSpan.FromDays(count);
		}

		public static DateTime TakeAwayWeek(this DateTime firstDateTime)
		{
			return firstDateTime - TimeSpan.FromDays(7);
		}

		public static DateTime TakeAwayMonth(this DateTime firstDateTime)
		{
			return firstDateTime - TimeSpan.FromDays(30);
		}
	}
}
