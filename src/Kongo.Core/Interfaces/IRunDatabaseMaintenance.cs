using System.Threading;
using System.Threading.Tasks;

namespace Kongo.Core.Interfaces
{
	public interface IRunDatabaseMaintenance
	{
		Task RunDatabaseMaintenance(CancellationToken stoppingToken);
	}
}
