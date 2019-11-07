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
	public class LeadersProcessorTests
	{
		private IProcessLeaders _processor;

		const string _leadersStream1 = "{[1]}";

		[Theory]
		[InlineData(_leadersStream1)]
		public async Task ProcessLeaders(string value)
		{
			var _sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = Path.GetRandomFileName() };
			var storage = new KongoDataStorage(_sqliteConfiguration);
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var nodeStats = await _processor.ProcessLeaders(value);
			storage.Database.EnsureDeleted();
			Assert.True(false);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

	}
}
