using Newtonsoft.Json;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System.Threading.Tasks;
using Kongo.Core.Helpers;
using Kongo.Core.DataServices;
using Microsoft.EntityFrameworkCore;
using System;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class NodeStatisticsProcessor : IProcessNodeStatistics
	{
		private readonly KongoDataStorage _database;

		public NodeStatisticsProcessor(KongoDataStorage database)
		{
			_database = database;
			_database.Database.EnsureCreated();
			_database.Database.Migrate();
		}
		/// <summary>
		/// Process Network statistics and summarize into ProcessedNodeStatisticsModel
		/// </summary>
		/// <param name="jsonNodeStatistics"></param>
		/// <returns>ProcessedNodeStatisticsModel</returns>
		public Task<NodeStatisticsModel> ProcessNodeStatistics(string jsonNodeStatistics)
		{
			Exceptions.ThrowIfNotJson(jsonNodeStatistics, "jsonNodeStatistics");

			var nodeStats = JsonConvert.DeserializeObject<NodeStatisticsModel>(jsonNodeStatistics);
			nodeStats.Timestamp = DateTimeOffset.UtcNow;
			_database.NodeStats.Add(nodeStats);
			_database.SaveChanges();

			return Task.FromResult(nodeStats);
		}
	}
}
