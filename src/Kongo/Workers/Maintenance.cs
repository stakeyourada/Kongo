using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kongo.Core.DataServices;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
		private readonly KongoDataStorage _database;

		public Maintenance(ILogger<Maintenance> logger, KongoOptions opts, KongoDataStorage database)
		{
			_logger = logger;
			_sb = new StringBuilder();
			_opts = opts;
			_database = database;
			_database.Database.EnsureCreated();
			_database.Database.Migrate();
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

					var nodeEntriesEnumerable = _database.NodeStatisticEntries.AsEnumerable();
					var fragmentEntriesEnumerable = _database.FragmentStatistics.AsEnumerable();
					var networkEntriesEnumerable = _database.NetworkStatistics.AsEnumerable();

					foreach (var entry in nodeEntriesEnumerable.Where(n => n.Timestamp < DateTimeOffset.UtcNow.AddDays(-3)))
					{
						_database.NodeStatisticEntries.Remove(entry);
						stoppingToken.ThrowIfCancellationRequested();
					}

					foreach (var entry in nodeEntriesEnumerable.Where(n => string.IsNullOrEmpty(n.LastBlockDate) ))
					{
						_database.NodeStatisticEntries.Remove(entry);
						stoppingToken.ThrowIfCancellationRequested();
					}

					foreach (var entry in fragmentEntriesEnumerable.Where(n => n.Timestamp < DateTimeOffset.UtcNow.AddDays(-3)))
					{
						_database.FragmentStatistics.Remove(entry);
						stoppingToken.ThrowIfCancellationRequested();
					}

					foreach (var entry in networkEntriesEnumerable.Where(n => n.Timestamp < DateTimeOffset.UtcNow.AddDays(-3)))
					{
						_database.NetworkStatistics.Remove(entry);
						stoppingToken.ThrowIfCancellationRequested();
					}

					await _database.SaveChangesAsync();

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
