using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessStake
	{
		Task<ProcessedStakeModel> ProcessStake(string jsonContent);
	}
}
