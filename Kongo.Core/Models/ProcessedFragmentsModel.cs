using System;
using System.Collections.Generic;
using System.Net;

namespace Kongo.Core.Models
{
	public class ProcessedFragmentsModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public long FragmentsReceviedFromRest { get; set; }
		public IEnumerable<FragmentModel> RestFragments { get; set; }
		public long FragmentsReceviedFromNetwork { get; set; }
		public IEnumerable<FragmentModel> NetworkFragments { get; set; }
		public long FragmentsInBlock { get; set; }
		public IEnumerable<FragmentModel> BlockFragments { get; set; }
		public long FragmentsPending { get; set; }
		public IEnumerable<FragmentModel> PendingFragments { get; set; }
		public long TotalFragments { get; set; }
	}
}
