using System;
using System.Collections.Generic;
using System.Text;

namespace Kongo.Core.Models
{
	public class NodeResourceUsageModel
	{
		public double NodeCpuUtilization { get; set; }
		public double NodeMemoryConsumption { get; set; }
		public double KongoCpuUtilization { get; set; }
		public double KongoMemoryConsumption { get; set; }
	}
}
