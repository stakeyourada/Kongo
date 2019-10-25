using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessNetworkStatistics
	{
		Task<ProcessedNetworkStatisticsModel> ProcessNetworkStatistics(string jsonNetworkStatistics);
	}
}
