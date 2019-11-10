using System;
using System.Collections.Generic;

namespace Kongo.Core.Models
{

	public class LeadersLogsModel
	{
		public DateTimeOffset Created_at_time { get; set; }
		public int Enclave_leader_id { get; set; }
		public string Scheduled_at_date { get; set; }
		public DateTimeOffset? Finished_at_time { get; set; }
		public DateTimeOffset Scheduled_at_time { get; set; }
		public DateTimeOffset? Wake_at_time { get; set; }
	}
}
