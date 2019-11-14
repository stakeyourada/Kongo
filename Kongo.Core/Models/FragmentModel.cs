using System;
using System.Runtime.CompilerServices;

namespace Kongo.Core.Models
{

	public class RejectedDetail
	{
		public string Reason { get; set; }
	}

	public class RejectedStatus
	{
		public RejectedDetail Rejected { get; set; }
	}

	public class InABlockDetail
	{
		public string Block { get; set; }
		public string Date { get; set; }
	}

	public class InABlockStatus
	{
		public InABlockDetail InABlock { get; set; }
	}

	public class RejectedFragmentModel
	{
		public string Fragment_id { get; set; }
		public DateTimeOffset Last_updated_at { get; set; }
		public DateTimeOffset Received_at { get; set; }
		public string Received_from { get; set; }
		public RejectedDetail Status { get; set; }
	}

	public class InBlockFragmentModel
	{
		public string Fragment_id { get; set; }
		public DateTimeOffset Last_updated_at { get; set; }
		public DateTimeOffset Received_at { get; set; }
		public string Received_from { get; set; }
		public InABlockDetail Status { get; set; }
	}

	public class PendingFragmentModel
	{
		public string Fragment_id { get; set; }
		public DateTimeOffset Last_updated_at { get; set; }
		public DateTimeOffset Received_at { get; set; }
		public string Received_from { get; set; }
		public string Status { get; set; }
	}

	public class FragmentModel
	{
		public string Fragment_id { get; set; }
		public DateTimeOffset Last_updated_at { get; set; }
		public DateTimeOffset Received_at { get; set; }
		public string Received_from { get; set; }
		public dynamic Status { get; set; }
	}
}
