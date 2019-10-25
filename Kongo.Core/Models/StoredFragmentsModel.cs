using System;

namespace Kongo.Core.Models
{
	public class StoredFragmentsModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public long FragmentsReceviedFromRest { get; set; }
		public long FragmentsReceviedFromNetwork { get; set; }
		public long FragmentsInBlock { get; set; }
		public long FragmentsPending { get; set; }
		public long TotalFragments { get; set; }
	}

}
