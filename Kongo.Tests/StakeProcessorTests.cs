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
	public class StakeProcessorTests
	{
		private IProcessStake _processor;

		const string _stakeStream1 = "{\"epoch\":85,\"stake\":{\"dangling\":0,\"pools\":[[\"e0c2ab61da2766c690a11f3283e3be0dc68ea8b4a7ae12eb792e9319b9f68e4b\",66850],[\"7780ce62a4916dc29b2dbf35c125b106fb177593e25c0e2fe4bed372b51cca7c\",0],[\"40812db2975f816656510336ac1fbb8a101f3df9e67b711f0d178a28361f82ac\",246285361044],[\"c8728726ad3521524c63b2e5c22236880f300cc758ff3d314a02fc91c63d49de\",0],[\"55ba41f01ad22ee98e4bcf7767a98ac845d8c77452403184dfda1eeaf7a2dac1\",0],[\"76e5180aff64302bce6b9a45074c10685004c580c3a7b2287071992a7a71a6c4\",66850],[\"956eae1cdb60db42c9909b80a37a8fb26e354ed2650ac2885ef2699ccc689b28\",103128166558],[\"e7685aca5347eca08834ad9916883482e015f8cce8da0d4a5dbd9d4e15f3faee\",0],[\"0a16ff59447df5534a2f267aba4c02fe7c371cda2122c6b836049e540a850239\",0],[\"d811222b904a72172ff145bd1d0a1923a943d45fc6c536244a1699c74a935bfc\",0],[\"22d213bf5b9220c47406832d7f543e0f1e5716a7c22b0457d8a1092008c4b5e5\",0],[\"48253eebea3a6d5ebce03a3dfac4a1281caab3748e50fe625aa6283cd7906505\",0],[\"62550ee71694fce07d2ed24e46c8cdfd7e1632e843e0f0f881835720e7395a56\",9762870278946],[\"9d466ae27d680d4f7383731c937a115ec0deb3e771bbe0fb2e96f24957d5170a\",0]],\"unassigned\":12869176873724}";

		[Theory]
		[InlineData(_stakeStream1)]
		public async Task ProcessStakeDistribution(string value)
		{
			var _sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = Path.GetRandomFileName() };
			var storage = new KongoDataStorage(_sqliteConfiguration);
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new StakeProcessor(storage, opts);
			var nodeStats = await _processor.ProcessStake(value);
			storage.Database.EnsureDeleted();
			Assert.True(false);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}


	}
}
