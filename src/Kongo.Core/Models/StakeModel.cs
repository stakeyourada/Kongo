using System;

namespace Kongo.Core.Models
{
	public class StakeDetails
	{
		public long Dangling { get; set; }
		public dynamic Pools { get; set; }
		public long Unassigned { get; set; }
	}

	public class StakeModel
	{
		public DateTimeOffset Timestamp { get; set; }
		public long Epoch { get; set; }
		public StakeDetails Stake { get; set; }
	}
}
