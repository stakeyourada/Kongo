namespace Kongo.Core.Models
{
	public enum TimeRangeEnum
	{
		FifteenMinutes = 15,
		ThirtyMinutes = 30,
		OneHour = 60,
		SixHours = 360,
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
					return 2; 
				case TimeRangeEnum.OneHour:
					return 4; 
				case TimeRangeEnum.SixHours:
					return 30; 
				case TimeRangeEnum.TwelveHours:
					return 60; 
				case TimeRangeEnum.OneDay:
					return 40; 
				case TimeRangeEnum.ThreeDays:
					return 120; 
				default:
					return 1;
			}
		}
	}
}
