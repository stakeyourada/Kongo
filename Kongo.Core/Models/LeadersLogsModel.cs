using System;

namespace Kongo.Core.Models
{
	public class BlockDetail
	{
		public string Block { get; set; }
		public long Chain_length { get; set; }
	}

	public class BlockStatus
	{
		public BlockDetail Block  { get; set; }
	}

	public class StoredLeadersLogsModel
	{
		public DateTimeOffset Created_at_time { get; set; }
		public int Enclave_leader_id { get; set; }
		public string Scheduled_at_date { get; set; }
		public DateTimeOffset? Finished_at_time { get; set; }
		public DateTimeOffset Scheduled_at_time { get; set; }
		public string Status { get; set; }
		public string? Block { get; set; }
		public long? Chain_length { get; set; }
		public DateTimeOffset? Wake_at_time { get; set; }		
	}

	public class LeadersLogsModel
	{
		public DateTimeOffset Created_at_time { get; set; }
		public int Enclave_leader_id { get; set; }
		public string Scheduled_at_date { get; set; }
		public DateTimeOffset? Finished_at_time { get; set; }
		public DateTimeOffset Scheduled_at_time { get; set; }
		public DateTimeOffset? Wake_at_time { get; set; }
		public dynamic Status { get; set; }
	}

}
