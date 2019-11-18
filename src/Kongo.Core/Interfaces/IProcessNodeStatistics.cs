using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessNodeStatistics
	{
		Task<NodeStatisticsModel> ProcessNodeStatistics(string jsonContent);
	}
}
