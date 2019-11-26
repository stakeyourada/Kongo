using Kongo.Core.DataServices;
using Kongo.Core.Helpers;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kongo.Core.DataProcessors
{
	/// <summary>
	/// Network Fragments processor
	/// </summary>
	public class StakePoolsProcessor : IProcessStakePools
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public StakePoolsProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			_opts = opts;
		}

		/// <summary>
		/// Process Network Fragments and summarize into ProcessedFragmentsModel
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedStakePoolsModel> ProcessStakePools(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			//var fragments = JsonConvert.DeserializeObject<List<FragmentModel>>(jsonFragments);

			var result = new ProcessedStakePoolsModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
			};

			//// SQL Lite can't map IEnum or dynamic objects, so trim to just the aggregate counts
			//_database.Leaders.Add(new StoredLeadersModel
			//{
			//	Timestamp = result.Timestamp				
			//});
			
			//_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}
	}
}
