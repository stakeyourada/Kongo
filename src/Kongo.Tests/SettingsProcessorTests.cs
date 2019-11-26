using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Kongo.Tests
{
	public class SettingsProcessorTests
	{
		private IProcessSettings _processor;

		const string _settingsStream1 = "{\"block0Hash\":\"cfd99bc54ebf44b44e72db7e2d48a40499888781e7628ea0fbf286bfd480ca58\",\"block0Time\":\"2019-11-07T22:41:46+00:00\",\"consensusVersion\":\"genesis\",\"currSlotStartTime\":\"2019-11-10T15:44:38+00:00\",\"fees\":{\"certificate\":10000,\"coefficient\":50,\"constant\":1000},\"maxTxsPerBlock\":255,\"slotDuration\":2,\"slotsPerEpoch\":7200}";

		[Theory]
		[InlineData(_settingsStream1)]
		public async Task ProcessSettings(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new SettingsProcessor(storage, opts);
			var settings = await _processor.ProcessSettings(value);
			storage.Database.EnsureDeleted();
			Assert.True(settings.Coefficient == 50);
		}


	}
}
