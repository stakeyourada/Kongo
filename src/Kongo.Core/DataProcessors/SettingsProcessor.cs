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
	public class SettingsProcessor : IProcessSettings
	{
		private readonly KongoDataStorage _database;
		private KongoOptions _opts;

		public SettingsProcessor(KongoDataStorage database, KongoOptions opts)
		{
			_database = database;
			_opts = opts;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jsonContent"></param>
		/// <returns></returns>
		public Task<ProcessedSettingsModel> ProcessSettings(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var result = JsonConvert.DeserializeObject<ProcessedSettingsModel>(jsonContent);
			result.Timestamp = DateTimeOffset.UtcNow;

			//query last settings, if changed save the new settings to the DB

			//// SQL Lite can't map IEnum or dynamic objects, so trim to just the aggregate counts
			//_database.Leaders.Add(new StoredLeadersModel
			//{
			//	Timestamp = result.Timestamp				
			//});
			
			//_database.SaveChanges();

			return Task.FromResult(result);
		}
	}
}
