using Kongo.Core.Models;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IProcessSettings
	{
		Task<StoredSettingsModel> ProcessSettings(string jsonContent);
	}
}
