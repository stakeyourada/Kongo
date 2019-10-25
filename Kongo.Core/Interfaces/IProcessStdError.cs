using System.Threading.Tasks;
using Kongo.Core.Models;

namespace Kongo.Core.Interfaces
{
	public interface IProcessStdError
	{
		Task IngestLogEntry(string logLine, int v);
		Task<ProcessedLogModel> ProcessIngestedLogs();
		Task<bool> DeleteDatabaseFile();
	}
}
