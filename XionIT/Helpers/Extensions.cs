using System;
using System.Collections.Generic;
using System.IO;
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

		public static DateTime RandomDate(DateTime startDate, DateTime endDate, Random rnd = null)
		{
			TimeSpan timeSpan = endDate - startDate;
			var randomTest = rnd ?? new Random();
			TimeSpan newSpan = new TimeSpan(0, randomTest.Next(0, (int)timeSpan.TotalMinutes), 0);
			DateTime newDate = startDate + newSpan;
			return newDate;
		}

	}

	public static class StringExtenstions
	{
		public static string Truncate(this string value, int maxLength, string overText = "...")
		{
			if (string.IsNullOrEmpty(value)) return value;
			return value.Length <= maxLength ? value : string.Format("{0}{1}", value.Substring(0, maxLength), overText);
		}

		public static string RandomString(int len = 8, Random rnd = null)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = rnd ?? new Random();
			return new string(Enumerable.Repeat(chars, len).Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static string RandomEmail(Random rnd = null)
		{
			Random r = rnd ?? new Random();
			return string.Format(@"{0}@{1}.com", RandomString(8, r), RandomString(5, r));
		}

	}

	public static class EnumerableExtensions
	{
		public static T PickRandom<T>(this IEnumerable<T> source)
		{
			return source.PickRandom(1).Single();
		}

		public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
		{
			return source.Shuffle().Take(count);
		}

		public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int minCount, int MaxCount, Random rnd = null)
		{
			var random = rnd ?? new Random();
			return source.Shuffle().Take(rnd.Next(minCount, MaxCount));
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.OrderBy(x => Guid.NewGuid());
		}
	}
}