namespace HCM.Application.Helpers
{
	public static class LeaveHelper
	{
		public static int GetPaidLeaveDays(DateTime startDate, DateTime endDate)
		{
			int paidLeaveDays = 0;
			// List of public holidays
			List<DateTime> publicHolidays = new ()
			{
				new DateTime(2025, 1, 1),
				new DateTime(2025, 3, 3),
				new DateTime(2025, 4, 18),
				new DateTime(2025, 4, 19),
				new DateTime(2025, 4, 20),
				new DateTime(2025, 4, 21),
				new DateTime(2025, 5, 1),
				new DateTime(2025, 5, 6),
				new DateTime(2025, 5, 26),
				new DateTime(2025, 9, 6),
				new DateTime(2025, 9, 22),
				new DateTime(2025, 12, 24),
				new DateTime(2025, 12, 25),
				new DateTime(2025, 12, 26),
			};

			for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
			{
				if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
					continue;

				if (publicHolidays.Contains(date))
					continue;

				paidLeaveDays++;
			}

			return paidLeaveDays;
		}
	}
}
