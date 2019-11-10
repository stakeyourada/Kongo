using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessSettings
	{
		Task<ProcessedSettingsModel> ProcessSettings(string jsonContent);
	}
}
