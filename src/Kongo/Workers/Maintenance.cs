using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kongo.Core.Interfaces;

namespace Kongo.Workers
{
	/// <summary>
	/// Data retention maintenance job
	/// </summary>
	public class Maintenance : BackgroundService
	{
		private readonly ILogger<Maintenance> _logger;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;
		private readonly IRunDatabaseMaintenance _databaseMaintenance;

		public Maintenance(ILogger<Maintenance> logger, KongoOptions opts, IRunDatabaseMaintenance databaseMaintenance)
		{
			_logger = logger;
			_sb = new StringBuilder();
			_opts = opts;
			_databaseMaintenance = databaseMaintenance;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					_sb.Clear();
					_sb.AppendLine($"Database Maintenance job started running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
					_sb.AppendLine();
					_sb.AppendLine($"Database path = {_opts.DatabasePath}");

					await _databaseMaintenance.RunDatabaseMaintenance(stoppingToken);

					_sb.AppendLine($"Database Maintenance job completed at: {DateTimeOffset.Now}");
					_sb.AppendLine();

					_logger.LogInformation(_sb.ToString());

					// wait 12 hours before rerunning this job
					await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);

					// if an exception occured try again every 5 minutes
					await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
				}
			}
		}
	}
}
