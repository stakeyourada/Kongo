using System;
using System.Collections.Generic;
using System.Text;

namespace Kongo.Core.Models
{
	public class LogIngestionModel
	{
		public long Id { get; set; }
		public string Msg { get; set; }
		public string Level { get; set; }
		public DateTimeOffset Ts { get; set; }
		public string Node_id { get; set; }
		public string Peer_addr { get; set; }
		public string Task { get; set; }
		public string Reason { get; set; }
	}
}
