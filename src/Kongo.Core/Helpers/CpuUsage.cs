using System;
using System.Diagnostics;

namespace Kongo.Core.Helpers
{
	// .NET Standard Process-based impl (cross-platform)
	internal class CpuUsage
	{
		private DateTimeOffset prevTime = DateTimeOffset.MinValue;
		private DateTimeOffset currTime = DateTimeOffset.MinValue;
		private TimeSpan prevTotalProcessorTime;
		private TimeSpan currTotalProcessorTime;

		public int GetCpuUsageProcess(Process p)
		{
			if (p == null || p.HasExited)
			{
				return 0;
			}

			if (this.prevTime == DateTimeOffset.MinValue)
			{
				this.prevTime = DateTimeOffset.UtcNow;
				this.prevTotalProcessorTime = p.TotalProcessorTime;
			}
			else
			{
				this.currTime = DateTimeOffset.UtcNow;
				this.currTotalProcessorTime = p.TotalProcessorTime;
				var currentUsage = (this.currTotalProcessorTime.TotalMilliseconds - this.prevTotalProcessorTime.TotalMilliseconds) / this.currTime.Subtract(this.prevTime).TotalMilliseconds;
				double cpuUsuage = currentUsage / Environment.ProcessorCount;
				this.prevTime = this.currTime;
				this.prevTotalProcessorTime = this.currTotalProcessorTime;
				return (int)(cpuUsuage * 100);
			}

			return 0;
		}
	}
}
