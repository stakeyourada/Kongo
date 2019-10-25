namespace Kongo.Core.Models
{
	public enum TimeRangeEnum
	{
		FifteenMinutes = 15,
		ThirtyMinutes = 30,
		OneHour = 60,
		TwelveHours = 720,
		OneDay = 1440,
		ThreeDays = 4320
	}

	public class TimeRangeMap
	{
		public static int GetGroupingIntervalFromTimeRange(TimeRangeEnum range)
		{
			switch(range)
			{
				case TimeRangeEnum.FifteenMinutes:
					return 1;
				case TimeRangeEnum.ThirtyMinutes:
					return 1;
				case TimeRangeEnum.OneHour:
					return 2;
				case TimeRangeEnum.TwelveHours:
					return 10;
				case TimeRangeEnum.OneDay:
					return 20;
				case TimeRangeEnum.ThreeDays:
					return 60;
				default:
					return 1;
			}
		}
	}
}
