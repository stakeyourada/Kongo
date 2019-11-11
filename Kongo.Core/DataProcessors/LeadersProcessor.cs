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
	public class LeadersProcessor : IProcessLeaders
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public LeadersProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			//_database.Database.EnsureCreated();
			_database.Database.Migrate();
			_opts = opts;
		}

		/// <summary>
		/// Process Network Fragments and summarize into ProcessedFragmentsModel
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedLeadersModel> ProcessLeaders(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");
			var leaders = JArray.Parse(jsonContent);

			var result = new ProcessedLeadersModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				Leaders = new List<int>()
			};

			foreach (var leader in leaders)
			{
				if (int.TryParse(leader.ToString(), out int leaderid))
				{
					result.Leaders.Add(leaderid);
				}
			}

			//// SQL Lite can't map IEnum or dynamic objects, so trim to just the aggregate counts
			//_database.Leaders.Add(new StoredLeadersModel
			//{
			//	Timestamp = result.Timestamp				
			//});

			//_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}

		public Task<ProcessedLeadersLogsModel> ProcessLeadersLogs(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");
			
			var leadersLogs = JsonConvert.DeserializeObject<List<LeadersLogsModel>>(jsonContent);

			var result = new ProcessedLeadersLogsModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				LeadersLogs = new List<LeadersLogsModel>()
			};

			foreach (var log in leadersLogs)
			{
				result.LeadersLogs.Add(log);
			}

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
