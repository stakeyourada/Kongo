using System;

namespace Kongo.Core.Models
{
	public class KongoStatusModel
	{
		public string PoolState { get; set; }

		public DateTimeOffset LastBlockReceivedAt { get; set; }

		public string CurrentBlockDate { get; set; }

		public long CurrentBlockHeight { get; set; }

		public TimeSpan PoolUptime { get; set; }

		public TimeRangeEnum CurrentChartTimeframe { get; set; }

		public KongoStatusModel ShallowCopy()
		{
			return (KongoStatusModel)this.MemberwiseClone();
		}
	}
}
