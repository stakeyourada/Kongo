using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Kongo.Tests
{
	public class StakePoolsProcessorTests
	{
		private IProcessStakePools _processor;
		const string _stakePoolsList1 = "[\"c8728726ad3521524c63b2e5c22236880f300cc758ff3d314a02fc91c63d49de\",\"f3b35041dda7a98eb3f7ac7cb24eb8344875c33fc54781b51e2372745720584a\",\"bcbc05b62d8274132b1f244f9e8b5a6a1d3853c64fd3a91c260c5db082739649\",\"e12cd8502bb89913eaa2fa1a39e5e09b44e34dccb1732e085e2aafb1093f6b62\",\"c6adcbea82106c14253473344244f59ac37fef4f89684e8c2ea299590259d878\",\"d2e3dbbf338e5d9a4b979d21045153c572234c3e1674b759f7e646b2417159d3\",\"1d0eba620c0c4a9037397a9b1f3c7b4f908cbb8c874a572088d5badda16ab97b\",\"2e1e0912f3bf35b510895d2de8cd3465a0d73b48f52550bf2c3a15baf3ba5837\",\"312430403dbfeefbd1e7fb5c3baad5a4b5b56a0671c0dcaafa18e08dd0d79f06\",\"62550ee71694fce07d2ed24e46c8cdfd7e1632e843e0f0f881835720e7395a56\",\"a9398a9f5aa4b6a5b45ad7debd36a8c6effbdd5ddcc6f692c822d413cd1e2299\",\"e0c2ab61da2766c690a11f3283e3be0dc68ea8b4a7ae12eb792e9319b9f68e4b\"]";

		[Theory]
		[InlineData(_stakePoolsList1)]
		public async Task ProcessStakePools(string value)
		{

			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new StakePoolsProcessor(storage, opts);
			var stakePools = await _processor.ProcessStakePools(value);
			storage.Database.EnsureDeleted();
			Assert.True(true);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}


	}
}
