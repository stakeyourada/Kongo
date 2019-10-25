using System;

namespace Kongo.Core.Models
{
	public class FragmentModel
	{
		public string Fragment_id { get; set; }
		public DateTimeOffset Last_updated_at { get; set; }
		public DateTimeOffset Received_at { get; set; }
		public string Received_from { get; set; }
		public dynamic Status { get; set; }
	}
}
