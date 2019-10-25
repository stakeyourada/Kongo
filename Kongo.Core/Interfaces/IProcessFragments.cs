using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessFragments
	{
		Task<ProcessedFragmentsModel> ProcessFragments(string jsonFragments);
	}
}
