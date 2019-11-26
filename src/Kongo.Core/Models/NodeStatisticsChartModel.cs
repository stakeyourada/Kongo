using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kongo.Core.Models
{
	public class NodeStatisticsChartModel
	{
		public DateTimeOffset Timestamp { get; set; }
		public long MaxBlockRecvCnt { get; set; }
		public long MaxLastBlockHeight { get; set; }
		public long MaxUpTime { get; set; }
		public long MinUpTime { get; set; }
		public DateTimeOffset LastBlockTime { get; set; }
		public double SkewInMinutes { get; set; }
	}
}
