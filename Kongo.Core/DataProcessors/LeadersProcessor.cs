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
using System.Net.Http.Formatting;
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
			_database.Database.EnsureCreated();
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

			var parsedLeadersLogs = new List<StoredLeadersLogsModel>();

			foreach (var leaderlog in leadersLogs)
			{
				var statusType = leaderlog.Status.GetType();

				if (statusType == typeof(string))
				{
					parsedLeadersLogs.Add(
						new StoredLeadersLogsModel
						{
							Created_at_time = leaderlog.Created_at_time,
							Enclave_leader_id = leaderlog.Enclave_leader_id,
							Scheduled_at_date = leaderlog.Scheduled_at_date,
							Finished_at_time = leaderlog.Finished_at_time,
							Scheduled_at_time = leaderlog.Scheduled_at_time,
							Status = (string)leaderlog.Status,
							Block = null,
							Chain_length = null,
							Wake_at_time = leaderlog.Wake_at_time
						});
					continue;
				}
				if (statusType == typeof(JObject))
				{
					var json = leaderlog.Status.ToString(Formatting.None);

					BlockStatus blockStatus = JsonConvert.DeserializeObject<BlockStatus>(json);

					if (blockStatus.Block != null)
					{
						parsedLeadersLogs.Add(
							new StoredLeadersLogsModel
							{
								Created_at_time = leaderlog.Created_at_time,
								Enclave_leader_id = leaderlog.Enclave_leader_id,
								Scheduled_at_date = leaderlog.Scheduled_at_date,
								Finished_at_time = leaderlog.Finished_at_time,
								Scheduled_at_time = leaderlog.Scheduled_at_time,
								Status = "Block",
								Block = blockStatus.Block.Block,
								Chain_length = blockStatus.Block.Chain_length,
								Wake_at_time = leaderlog.Wake_at_time
							});
					}					
				}
			}

			var result = new ProcessedLeadersLogsModel()
			{
				Timestamp = DateTimeOffset.UtcNow,
				LeadersLogsJson = JsonConvert.SerializeObject(parsedLeadersLogs, Formatting.None)
			};

			// SQL Lite can't map IEnum or dynamic objects, so store as json
			_database.LeadersLogs.Add(result);
			_database.SaveChanges();

			// return ProcessedFragmentsModel result
			return Task.FromResult(result);
		}
	}
}
