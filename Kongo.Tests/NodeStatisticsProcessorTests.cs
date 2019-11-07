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
	public class NodeStatisticsProcessorTests
	{
		private IProcessNodeStatistics _processor;

		const string _nodeStatsStream1 = "{\"blockRecvCnt\":86441,\"lastBlockDate\":\"240.19402\",\"lastBlockFees\":0,\"lastBlockHash\":\"55ce294adaae67fc7e954cd6b08a63843367a70e2518968dbaade65e4d7a5536\",\"lastBlockHeight\":\"160287\",\"lastBlockSum\":0,\"lastBlockTime\":null,\"lastBlockTx\":0,\"txRecvCnt\":12,\"uptime\":17166}";
		const string _nodeStatsStream2 = "{\"blockRecvCnt\":91987,\"lastBlockDate\":\"240.19948\",\"lastBlockFees\":0,\"lastBlockHash\":\"260392dee7be1fd279d8141e5c97768dbd7224fa74dc6baac5b75a88c237db1b\",\"lastBlockHeight\":\"160438\",\"lastBlockSum\":0,\"lastBlockTime\":\"2019-10-20T19:00:13+00:00\",\"lastBlockTx\":0,\"txRecvCnt\":12,\"uptime\":17227}";
		const string _nodeStatsStream3 = "{\"blockRecvCnt\":92843,\"lastBlockDate\":\"240.19984\",\"lastBlockFees\":0,\"lastBlockHash\":\"f1da8d231a0a39eaa230b56ad3b3a57a88eab6be3d6c9e849d88f3f718edd3ae\",\"lastBlockHeight\":\"160449\",\"lastBlockSum\":0,\"lastBlockTime\":\"2019-10-20T19:00:43+00:00\",\"lastBlockTx\":0,\"txRecvCnt\":12,\"uptime\":17257}";
		const string _nodeStatsStream4 = "{\"blockRecvCnt\":93850,\"lastBlockDate\":\"240.20023\",\"lastBlockFees\":0,\"lastBlockHash\":\"cc119814d28a0f54bcea5fcb9f5fc7db566ef439a38a619d731c6adfb1c60ea5\",\"lastBlockHeight\":\"160459\",\"lastBlockSum\":0,\"lastBlockTime\":\"2019-10-20T19:01:13+00:00\",\"lastBlockTx\":0,\"txRecvCnt\":12,\"uptime\":17287}";
		const string _nodeStatsStream5 = "{\"blockRecvCnt\":94775,\"lastBlockDate\":\"240.20028\",\"lastBlockFees\":0,\"lastBlockHash\":\"8a9cde404475ea166ed3f4e63861ab0a04b96442b7f271e3292e75a38657a6de\",\"lastBlockHeight\":\"160460\",\"lastBlockSum\":0,\"lastBlockTime\":\"2019-10-20T19:01:43+00:00\",\"lastBlockTx\":0,\"txRecvCnt\":12,\"uptime\":17317}";

		[Theory]
		[InlineData(_nodeStatsStream1)]
		public async Task ProcessNodeStatisticStreamWithNullLastBlockTime(string value)
		{
			var _sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = Path.GetRandomFileName() };
			var storage = new KongoDataStorage(_sqliteConfiguration);
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new NodeStatisticsProcessor(storage, opts);
			var nodeStats = await _processor.ProcessNodeStatistics(value);
			Assert.True(nodeStats != null, "nodeStats == null");
			Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

		[Theory]		
		[InlineData(_nodeStatsStream2)]
		public async Task ProcessValidNodeStatisticStream(string value)
		{
			var _sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = Path.GetRandomFileName() };
			var storage = new KongoDataStorage(_sqliteConfiguration);
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new NodeStatisticsProcessor(storage, opts);
			var nodeStats = await _processor.ProcessNodeStatistics(value);
			Assert.True(nodeStats != null, "nodeStats == null");
			Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}"); 
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("this is not a json formatted string")]
		public void InvalidNodeStatisticStream_Throws_ArgumentException(string value)
		{
			var _sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = Path.GetRandomFileName() };
			var storage = new KongoDataStorage(_sqliteConfiguration);
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new NodeStatisticsProcessor(storage, opts);
			storage.Database.EnsureDeleted();
			Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessNodeStatistics(value));
		}

	}
}
