using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class DatabaseMaintenance : IRunDatabaseMaintenance
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public DatabaseMaintenance(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			_database.Database.Migrate();
			_opts = opts;
		}

		public async Task RunDatabaseMaintenance(CancellationToken stoppingToken)
		{
			var nodeEntriesEnumerable = _database.NodeStatisticEntries.AsEnumerable();
			var fragmentEntriesEnumerable = _database.FragmentStatistics.AsEnumerable();
			var networkEntriesEnumerable = _database.NetworkStatistics.AsEnumerable();

			foreach (var entry in nodeEntriesEnumerable.Where(n => n.Timestamp < DateTimeOffset.UtcNow.AddDays(-3)))
			{
				_database.NodeStatisticEntries.Remove(entry);
				stoppingToken.ThrowIfCancellationRequested();
			}

			foreach (var entry in nodeEntriesEnumerable.Where(n => string.IsNullOrEmpty(n.LastBlockDate)))
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
		}
	}
}
