using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessResourceUsage
	{
		Task ProcessResourceUsage(NodeResourceUsageModel resourceUsage);
	}
}
