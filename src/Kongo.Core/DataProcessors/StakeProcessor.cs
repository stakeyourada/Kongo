using Kongo.Core.DataServices;
using Kongo.Core.Helpers;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class StakeProcessor : IProcessStake
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public StakeProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			_opts = opts;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedStakeModel> ProcessStake(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var poolDistibution = new List<PoolDistribution>();

			var stakeModel = JsonConvert.DeserializeObject<StakeModel>(jsonContent);

			var pools = (JArray)stakeModel.Stake.Pools;
			foreach (var pool in pools)
			{
				var distro = new PoolDistribution();
				foreach (var detail in pool)
				{
					var t = detail.Type;
					if (detail.Type.Equals(JTokenType.String))
					{
						distro.PoolId = (string)detail;
					} else
					{
						distro.AdaStaked = (long)detail;
					}
				}
				poolDistibution.Add(distro);
			}

			//// SQLite can't map IEnum or dynamic objects, so convert pooldistribtion list back to json to store
			var poolDistributionJson = JsonConvert.SerializeObject(poolDistibution, Formatting.None);

			var result = new ProcessedStakeModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				Epoch = stakeModel.Epoch,
				Dangling = stakeModel.Stake.Dangling,
				Unassigned = stakeModel.Stake.Unassigned,
				PoolDistributionJson = poolDistributionJson
			};

			_database.StakeDistribution.Add(result);
			
			_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}
	}
}
