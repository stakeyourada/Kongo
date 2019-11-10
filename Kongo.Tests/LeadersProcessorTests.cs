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

		const string _leadersStream0 = "[]";
		const string _leadersStream1 = "[1]";
		const string _leadersStream2 = "[1,2,3]";

		
		const string _leadersLogsStream0 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T22:18:12.546403700+00:00\",\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"wake_at_time\":\"2019-11-09T22:18:12.546403700+00:00\"}]";
		const string _leadersLogsStream1 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T22:18:12.546403700+00:00\",\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"wake_at_time\":\"2019-11-09T22:18:12.546403700+00:00\"},{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T20:24:30.259527700+00:00\",\"scheduled_at_date\":\"11.3082\",\"scheduled_at_time\":\"2019-11-09T20:24:30+00:00\",\"wake_at_time\":\"2019-11-09T20:24:30.259527700+00:00\"}]";
		const string _leadersLogsStream2 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":null,\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"wake_at_time\":null},{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T20:24:30.259527700+00:00\",\"scheduled_at_date\":\"11.3082\",\"scheduled_at_time\":\"2019-11-09T20:24:30+00:00\",\"wake_at_time\":\"2019-11-09T20:24:30.259527700+00:00\"}]";

		/*
			http://127.0.0.1:3101/api/v0/leaders
			StatusCode: 200, ReasonPhrase: 'OK', Version: 1.1, Content: System.Net.Http.HttpConnectionResponseContent, Headers:
			{
			  Date: Thu, 07 Nov 2019 23:10:18 GMT
			  Content-Length: 3
			  Content-Type: application/json
			}
			[1]

			http://127.0.0.1:3101/api/v0/leaders/logs
			[
				{
				"created_at_time": "2019-11-09T18:41:48.716930+00:00",
				"enclave_leader_id": 1,
				"finished_at_time": "2019-11-09T22:18:12.546403700+00:00",
				"scheduled_at_date": "11.6493",
				"scheduled_at_time": "2019-11-09T22:18:12+00:00",
				"wake_at_time": "2019-11-09T22:18:12.546403700+00:00"
				}
			]

		 */

		[Theory]
		[InlineData(_leadersStream0)]
		public async Task ProcessLeadersNullList(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeaders(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.Leaders.Count == 0);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

		[Theory]
		[InlineData(_leadersStream1)]
		[InlineData(_leadersStream2)]
		public async Task ProcessLeaders(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeaders(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.Leaders.Count > 0);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

		[Theory]
		[InlineData(_leadersLogsStream0)]
		[InlineData(_leadersLogsStream1)]
		public async Task ProcessLeadersLogs(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeadersLogs(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.LeadersLogs.Count > 0);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

		[Theory]
		[InlineData(_leadersLogsStream2)]
		public async Task ProcessLeadersLogsWithNullDates(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeadersLogs(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.LeadersLogs.Count > 0);
			//Assert.True(nodeStats != null, "nodeStats == null");
			//Assert.True(nodeStats.BlockRecvCnt > 0, $"BlockRecvCnt = {nodeStats.BlockRecvCnt}");
			//Assert.True(nodeStats.LastBlockTime != default, $"LastBlockTime = {nodeStats.LastBlockTime}");
		}

	}
}
