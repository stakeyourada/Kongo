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

	public class DisplayedLeadersLogsModel
	{
		public string Created_at_time { get; set; }
		public string Enclave_leader_id { get; set; }
		public string Scheduled_at_date { get; set; }
		public string Finished_at_time { get; set; }
		public string Scheduled_at_time { get; set; }
		public string Status { get; set; }
		public string Block { get; set; }
		public string Chain_length { get; set; }
		public string Wake_at_time { get; set; }
	}


	public class StoredLeadersLogsModel 
	{
		public DisplayedLeadersLogsModel ConvertToDisplayedLeadersLogsModel()
		{
			return new DisplayedLeadersLogsModel
			{
				Created_at_time = this.Created_at_time.ToString("u"),
				Enclave_leader_id = this.Enclave_leader_id.ToString(),
				Scheduled_at_date = this.Scheduled_at_date,
				Finished_at_time = this.Finished_at_time.HasValue ? this.Finished_at_time.Value.ToString("u") : "",
				Scheduled_at_time = this.Scheduled_at_time.ToString("u"),
				Status = this.Status,
				Block = this.Block,
				Chain_length = this.Chain_length.HasValue ? this.Chain_length.Value.ToString() : "",
				Wake_at_time = this.Wake_at_time.HasValue ? this.Wake_at_time.Value.ToString("u") : ""
			};
		}

		public DateTimeOffset Created_at_time { get; set; }
		public int Enclave_leader_id { get; set; }
		public string Scheduled_at_date { get; set; }
		public DateTimeOffset? Finished_at_time { get; set; }
		public DateTimeOffset Scheduled_at_time { get; set; }
		public string Status { get; set; }
		public string Block { get; set; }
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
