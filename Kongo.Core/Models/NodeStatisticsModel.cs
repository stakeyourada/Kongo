using System;

namespace Kongo.Core.Models
{
	public class NodeStatisticsModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public long BlockRecvCnt { get; set; }
		public string LastBlockDate { get; set; }
		public long LastBlockFees { get; set; }
		public string LastBlockHash { get; set; }
		public string LastBlockHeight { get; set; }
		public long lastBlockSum { get; set; }
		public DateTimeOffset? LastBlockTime { get; set; }
		public long LastBlockTx { get; set; }
		public long TxRecvCnt { get; set; }
		public long Uptime { get; set; }
		public string State { get; set; }
	}
}
