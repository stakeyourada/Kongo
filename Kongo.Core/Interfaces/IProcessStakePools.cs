using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessStakePools
	{
		Task<ProcessedStakePoolsModel> ProcessStakePools(string jsonContent);
	}
}
