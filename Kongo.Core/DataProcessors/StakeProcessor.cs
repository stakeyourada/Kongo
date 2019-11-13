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
	public class StakeProcessor : IProcessStake
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public StakeProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			_database.Database.EnsureCreated();
			_database.Database.Migrate();
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

			//var fragments = JsonConvert.DeserializeObject<List<StakeModel>>(jsonStake);

			var result = new ProcessedStakeModel()
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
