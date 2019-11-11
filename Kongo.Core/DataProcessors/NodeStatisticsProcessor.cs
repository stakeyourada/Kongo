using Newtonsoft.Json;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System.Threading.Tasks;
using Kongo.Core.Helpers;
using Kongo.Core.DataServices;
using Microsoft.EntityFrameworkCore;
using System;
using SQLitePCL;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class NodeStatisticsProcessor : IProcessNodeStatistics
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public NodeStatisticsProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			//_database.Database.EnsureCreated();
			_database.Database.Migrate();
			_opts = opts;
		}
		/// <summary>
		/// Process Network statistics and summarize into ProcessedNodeStatisticsModel
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns>ProcessedNodeStatisticsModel</returns>
		public Task<NodeStatisticsModel> ProcessNodeStatistics(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var nodeStats = JsonConvert.DeserializeObject<NodeStatisticsModel>(jsonContent);
			
			if (!nodeStats.LastBlockTime.HasValue)
				nodeStats.LastBlockTime = _opts.ApplicationStartedOn;

			nodeStats.Timestamp = DateTimeOffset.UtcNow;
			_database.NodeStatisticEntries.Add(nodeStats);
			_database.SaveChanges();

			return Task.FromResult(nodeStats);
		}
	}
}
