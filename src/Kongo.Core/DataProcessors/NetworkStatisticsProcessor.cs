using Newtonsoft.Json;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kongo.Core.Helpers;
using System;
using Kongo.Core.DataServices;
using Microsoft.EntityFrameworkCore;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Statistics processor
	/// </summary>
	public class NetworkStatisticsProcessor : IProcessNetworkStatistics
	{
		private readonly KongoDataStorage _database;

		public NetworkStatisticsProcessor(KongoDataStorage database)
		{
			_database = database;
			_database.Database.EnsureCreated();
			_database.Database.Migrate();
		}

		/// <summary>
		/// Process Network Fragments and summarize into ProcessedFragmentsModel
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedNetworkStatisticsModel> ProcessNetworkStatistics(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var networkStats = JsonConvert.DeserializeObject<List<NetworkStatisticsModel>>(jsonContent);

			var lastBlockReceivedAt = networkStats.Max(x => x.LastBlockReceived).HasValue ? networkStats.Max(x => x.LastBlockReceived).Value : default;
			var lastFragmentReceivedAt = networkStats.Max(x => x.LastFragmentReceived).HasValue ? networkStats.Max(x => x.LastFragmentReceived).Value : default;
			var lastGossipReceivedAt = networkStats.Max(x => x.LastGossipReceived).HasValue ? networkStats.Max(x => x.LastGossipReceived).Value : default;
			var blocksReceivedInPast30Min = networkStats.Where(x => x.LastBlockReceived > DateTimeOffset.UtcNow.AddMinutes(-30)).Count();
			var fragmentsReceivedInPast30Min = networkStats.Where(x => x.LastFragmentReceived > DateTimeOffset.UtcNow.AddMinutes(-30)).Count();
			var gossipReceivedInPast30Min = networkStats.Where(x => x.LastGossipReceived > DateTimeOffset.UtcNow.AddMinutes(-30)).Count();

			var result = new ProcessedNetworkStatisticsModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				TotalEstablishedConnections = networkStats.Count(),
				LastBlockReceivedAt = lastBlockReceivedAt,
				LastFragmentReceivedAt = lastFragmentReceivedAt,
				LastGossipReceivedAt = lastGossipReceivedAt,
				BlocksReceivedInPast30Min = blocksReceivedInPast30Min,
				FragmentsReceivedInPast30Min = fragmentsReceivedInPast30Min,
				GossipReceivedInPast30Min = gossipReceivedInPast30Min
			};

			_database.NetworkStatistics.Add(result);
			_database.SaveChanges();

			return Task.FromResult(result);
		}
	}
}
