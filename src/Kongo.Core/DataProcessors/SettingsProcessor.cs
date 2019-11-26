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
		public Task<StoredSettingsModel> ProcessSettings(string jsonContent)
		{
			Exceptions.ThrowIfNotJson(jsonContent, "jsonContent");

			var result = JsonConvert.DeserializeObject<ProcessedSettingsModel>(jsonContent);
			result.Timestamp = DateTimeOffset.UtcNow;

			//query last settings, if changed save the new settings to the DB

			var settings = GetCurrentSettings();

			if (result.CompareTo(settings) > 0)
			{
				_database.Settings.Add(new StoredSettingsModel
				{
					Timestamp = result.Timestamp,
					Block0Hash = result.Block0Hash,
					Block0Time = result.Block0Time,
					ConsensusVersion = result.ConsensusVersion,
					CurrSlotStartTime = result.CurrSlotStartTime,
					Certificate = result.Fees.Certificate,
					Coefficient = result.Fees.Coefficient,
					Constant = result.Fees.Constant,
					MaxTxsPerBlock = result.MaxTxsPerBlock,
					SlotDuration = result.SlotDuration,
					SlotsPerEpoch = result.SlotsPerEpoch
				});

				_database.SaveChanges();
			}

			return Task.FromResult(GetCurrentSettings());
		}

		private StoredSettingsModel GetCurrentSettings()
		{
			var records = _database.Settings.AsEnumerable();
			if (records.Skip(1).Any())
			{
				return records.OrderBy(r => r.Id).Last();
			} else
			{
				return records.FirstOrDefault();
			}
		}

	}
}
