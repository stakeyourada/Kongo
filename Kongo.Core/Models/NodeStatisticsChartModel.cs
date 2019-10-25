using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kongo.Core.Models
{
	public class NodeStatisticsChartModel
	{
		public DateTimeOffset Timestamp { get; set; }
		public long BlockRecvCnt { get; set; }
		public long LastBlockHeight { get; set; }
		public long UpTime { get; set; }
		public DateTimeOffset LastBlockTime { get; set; }
	}
}
