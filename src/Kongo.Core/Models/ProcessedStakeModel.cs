using System;

namespace Kongo.Core.Models
{
	public class PoolDistribution
	{
		public string PoolId { get; set; }
		public long AdaStaked { get; set; }
	}

	public class ProcessedStakeModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public long Epoch { get; set; }
		public long Dangling { get; set; }
		public string PoolDistributionJson { get; set; }
		public long Unassigned { get; set; }
	}
}
