using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XionIT
{
	public static class DateTimeExtensions
	{
		public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
		{
			int diff = dt.DayOfWeek - startOfWeek;
			if (diff < 0)
				diff += 7;

			return dt.AddDays(-1 * diff).Date;
		}

		public static DateTime FirstDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, 1);
		}

		public static int DaysInMonth(this DateTime value)
		{
			return DateTime.DaysInMonth(value.Year, value.Month);
		}

		public static DateTime LastDayOfMonth(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.DaysInMonth());
		}
	}
}