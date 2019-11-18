using System;

namespace Kongo.Core.Models
{
	public class KongoStatusModel
	{
		public string PoolState { get; set; }

		public DateTimeOffset LastBlockReceivedAt { get; set; }

		public long CurrentBlockHeight { get; set; }

		public TimeSpan PoolUptime { get; set; }

		public TimeRangeEnum CurrentChartTimeframe { get; set; }
	}
}
