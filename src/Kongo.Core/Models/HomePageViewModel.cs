using System;
using System.Collections.Generic;
using System.Text;

namespace Kongo.Core.Models
{
	public class HomePageViewModel
	{
		public HomePageViewModel()
		{
			ProcessedLeadersLogs = new List<StoredLeadersLogsModel>();
		}

		public KongoStatusModel KongoStatus { get; set; }
		public StoredFragmentsModel ProcessedFragments { get; set; }
		public List<StoredLeadersLogsModel> ProcessedLeadersLogs { get; set; }
		public ProcessedLeadersModel ProcessedLeaders { get; set; }
		public ProcessedLogModel ProcessedLog { get; set; }
		public ProcessedNetworkStatisticsModel ProcessedNetworkStatistics { get; set; }
		public ProcessedStakeModel ProcessedStake { get; set; }
		public ProcessedStakePoolsModel ProcessedStakePools { get; set; }
		public NodeStatisticsModel NodeStatistics { get; set; }
		public NodeResourceUsageModel NodeResourceUsage { get; set; }
	}
}
