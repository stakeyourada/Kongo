using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
		
		const string _leadersLogsStream0 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T22:18:12.546403700+00:00\",\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"status\":\"Pending\",\"wake_at_time\":\"2019-11-09T22:18:12.546403700+00:00\"}]";
		const string _leadersLogsStream1 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T22:18:12.546403700+00:00\",\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"status\":\"Pending\",\"wake_at_time\":\"2019-11-09T22:18:12.546403700+00:00\"},{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T20:24:30.259527700+00:00\",\"scheduled_at_date\":\"11.3082\",\"scheduled_at_time\":\"2019-11-09T20:24:30+00:00\",\"status\":\"Pending\",\"wake_at_time\":\"2019-11-09T20:24:30.259527700+00:00\"}]";
		const string _leadersLogsStream2 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":null,\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"status\":\"Pending\",\"wake_at_time\":null},{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T20:24:30.259527700+00:00\",\"scheduled_at_date\":\"11.3082\",\"scheduled_at_time\":\"2019-11-09T20:24:30+00:00\",\"status\":\"Pending\",\"wake_at_time\":\"2019-11-09T20:24:30.259527700+00:00\"}]";
		const string _leadersLogsStream3 = "[{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T22:18:12.546403700+00:00\",\"scheduled_at_date\":\"11.6493\",\"scheduled_at_time\":\"2019-11-09T22:18:12+00:00\",\"status\":\"Pending\",\"wake_at_time\":\"2019-11-09T22:18:12.546403700+00:00\"},{\"created_at_time\":\"2019-11-09T18:41:48.716930+00:00\",\"enclave_leader_id\":1,\"finished_at_time\":\"2019-11-09T20:24:30.259527700+00:00\",\"scheduled_at_date\":\"11.3082\",\"scheduled_at_time\":\"2019-11-09T20:24:30+00:00\",\"status\":{\"block\":{\"block\":\"asd897asdkjhasd987asdkjhasd87asdkjhasd987\",\"chain_length\":5088}},\"wake_at_time\":\"2019-11-09T20:24:30.259527700+00:00\"}]";

		[Theory]
		[InlineData(_leadersStream0)]
		public async Task ProcessLeadersNullList(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeaders(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.Leaders.Count == 0);
		}

		[Theory]
		[InlineData(_leadersStream1)]
		[InlineData(_leadersStream2)]
		public async Task ProcessLeaders(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeaders(value);
			storage.Database.EnsureDeleted();
			Assert.True(processedLeaders.Leaders.Count > 0);
		}

		[Theory]
		[InlineData(_leadersLogsStream0)]
		[InlineData(_leadersLogsStream1)]
		[InlineData(_leadersLogsStream3)]
		public async Task ProcessLeadersLogs(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeadersLogs(value);
			storage.Database.EnsureDeleted();

			var leadersLogs = JsonConvert.DeserializeObject<List<StoredLeadersLogsModel>>(processedLeaders.LeadersLogsJson);

			Assert.True(leadersLogs.Count > 0);
			Assert.False(leadersLogs[0].Scheduled_at_date.Equals(default));
		}

		[Theory]
		[InlineData(_leadersLogsStream2)]
		public async Task ProcessLeadersLogsWithNullDates(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			storage.Database.EnsureCreated();
			var opts = new KongoOptions() { ApplicationStartedOn = DateTimeOffset.UtcNow };
			_processor = new LeadersProcessor(storage, opts);
			var processedLeaders = await _processor.ProcessLeadersLogs(value);
			storage.Database.EnsureDeleted();
			var leadersLogs = JsonConvert.DeserializeObject<List<LeadersLogsModel>>(processedLeaders.LeadersLogsJson);

			Assert.True(leadersLogs.Count > 0);
			Assert.False(leadersLogs[0].Scheduled_at_date.Equals(default));
		}

	}
}
