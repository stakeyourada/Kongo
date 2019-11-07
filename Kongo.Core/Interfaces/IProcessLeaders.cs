using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessLeaders
	{
		Task<ProcessedLeadersModel> ProcessLeaders(string leadersContent);
	}
}
