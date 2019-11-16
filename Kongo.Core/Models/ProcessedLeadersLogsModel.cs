using System;
using System.Collections.Generic;

namespace Kongo.Core.Models
{
	public class ProcessedLeadersLogsModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string LeadersLogsJson { get; set; }
	}
}
