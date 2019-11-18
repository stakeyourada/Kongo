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

	public class StdErrorLogProcessorTests
	{
		private IProcessStdError _processor;

		/*
 

	 */
		const string _msg01 = "{\"msg\":\"failed to connect to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:47.298474400+00:00\",\"node_id\":\"55bc7212c36659baf4fe96f71026e83b817f86ef84f0cbce5878440fd8551d1d\",\"peer_addr\":\"108.61.210.25:3000\",\"task\":\"network\",\"reason\":\"connection failed: HTTP/2.0 connection error\"}";
		const string _msg02 = "{\"msg\":\"incoming P2P connection on 10.0.1.4:5253\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:48.268569600+00:00\",\"peer_addr\":\"168.63.129.16:59605\",\"task\":\"network\"}";
		const string _msg03 = "{\"msg\":\"incoming P2P HTTP/2 connection error\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:48.268569600+00:00\",\"peer_addr\":\"168.63.129.16:59568\",\"task\":\"network\",\"reason\":\"connection error: An existing connection was forcibly closed by the remote host. (os error 10054)\"}";
		const string _msg04 = "{\"msg\":\"incoming P2P HTTP/2 connection error\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:50.428246700+00:00\",\"peer_addr\":\"168.63.129.16:59605\",\"task\":\"network\",\"reason\":\"connection error: An existing connection was forcibly closed by the remote host. (os error 10054)\"}";
		const string _msg05 = "{\"msg\":\"incoming P2P connection on 10.0.1.4:5253\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:50.428246700+00:00\",\"peer_addr\":\"168.63.129.16:59649\",\"task\":\"network\"}";
		const string _msg06 = "{\"msg\":\"incoming P2P connection closed\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:53.465768600+00:00\",\"peer_addr\":\"3.24.176.165:48264\",\"task\":\"network\"}";
		const string _msg07 = "{\"msg\":\"connecting to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:55.500958300+00:00\",\"node_id\":\"130710d44c317a6d91a484470c24bb34510a6c7ebc8bd263ab1fe96c7a471ee6\",\"peer_addr\":\"51.154.214.163:3000\",\"task\":\"network\"}";
		const string _msg08 = "{\"msg\":\"connecting to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:55.829075+00:00\",\"node_id\":\"55bc7212c36659baf4fe96f71026e83b817f86ef84f0cbce5878440fd8551d1d\",\"peer_addr\":\"108.61.210.25:3000\",\"task\":\"network\"}";
		const string _msg09 = "{\"msg\":\"failed to connect to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:57.033464+00:00\",\"node_id\":\"130710d44c317a6d91a484470c24bb34510a6c7ebc8bd263ab1fe96c7a471ee6\",\"peer_addr\":\"51.154.214.163:3000\",\"task\":\"network\",\"reason\":\"connection failed: HTTP/2.0 connection error\"}";
		const string _msg10 = "{\"msg\":\"failed to connect to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:57.330347600+00:00\",\"node_id\":\"55bc7212c36659baf4fe96f71026e83b817f86ef84f0cbce5878440fd8551d1d\",\"peer_addr\":\"108.61.210.25:3000\",\"task\":\"network\",\"reason\":\"connection failed: HTTP/2.0 connection error\"}";
		const string _msg11 = "{\"msg\":\"incoming P2P connection on 10.0.1.4:5253\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:58.113332400+00:00\",\"peer_addr\":\"168.63.129.16:59678\",\"task\":\"network\"}";
		const string _msg12 = "{\"msg\":\"incoming P2P HTTP/2 connection error\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:58.129046100+00:00\",\"peer_addr\":\"168.63.129.16:59649\",\"task\":\"network\",\"reason\":\"connection error: An existing connection was forcibly closed by the remote host. (os error 10054)\"}";
		const string _msg13 = "{\"msg\":\"failed to connect to peer\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:56:58.129046100+00:00\",\"node_id\":\"7468c5e8cc3e937f7f428c92f21c336380e78d599b02e224c3ae815449ef7278\",\"peer_addr\":\"51.15.106.170:3001\",\"task\":\"network\",\"reason\":\"connection failed: HTTP/2.0 connection error\"}";
		const string _msg14 = "{\"msg\":\"incoming P2P HTTP/2 connection error\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:57:00.473459100+00:00\",\"peer_addr\":\"168.63.129.16:59678\",\"task\":\"network\",\"reason\":\"connection error: An existing connection was forcibly closed by the remote host. (os error 10054)\"}";
		const string _msg15 = "{\"msg\":\"incoming P2P connection on 10.0.1.4:5253\",\"level\":\"INFO\",\"ts\":\"2019-10-21T02:57:00.473459100+00:00\",\"peer_addr\":\"168.63.129.16:59719\",\"task\":\"network\"}";

		[Theory]
		[InlineData(_msg01)]
		[InlineData(_msg02)]
		[InlineData(_msg03)]
		[InlineData(_msg04)]
		[InlineData(_msg05)]
		[InlineData(_msg06)]
		[InlineData(_msg07)]
		[InlineData(_msg08)]
		[InlineData(_msg09)]
		[InlineData(_msg10)]
		[InlineData(_msg11)]
		[InlineData(_msg12)]
		[InlineData(_msg13)]
		[InlineData(_msg14)]
		[InlineData(_msg15)]
		public async Task ProcessValidStdErrStream(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new StdErrorProcessor(storage);
			await _processor.IngestLogEntry(value, 1);
			var logs = await _processor.ProcessIngestedLogs();

			Assert.True(await _processor.DeleteDatabaseFile());
			//Assert.True(fragments != null, "fragements == null");
			//Assert.True(fragments.FragmentsReceviedFromNetwork > 0, $"FragmentsReceviedFromNetwork = {fragments.FragmentsReceviedFromNetwork}");
			//Assert.True(fragments.FragmentsReceviedFromRest == 0, $"FragmentsReceviedFromRest = {fragments.FragmentsReceviedFromRest}");
			//if (value.Contains("08c7adcab95ccba8ab7a4828061363e7cb770ae24d1795f2895e37d1e6f5f1fa"))
			//{
			//	Assert.True(fragments.FragmentsInBlock > 0, $"FragmentsInBlock = {fragments.FragmentsInBlock}");
			//	Assert.True(fragments.FragmentsPending > 0, $"PendingFragments = {fragments.FragmentsPending}");
			//}
			//else
			//{
			//	Assert.True(fragments.FragmentsInBlock == 0, $"FragmentsInBlock = {fragments.FragmentsInBlock}");
			//	Assert.True(fragments.FragmentsPending > 0, $"PendingFragments = {fragments.FragmentsPending}");
			//}
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("this is not a json formatted string")]
		public void InvalidStdErrStream_Throws_ArgumentException(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new StdErrorProcessor(storage);
			Assert.ThrowsAsync<ArgumentException>(() => _processor.IngestLogEntry(value, 1));
			var result = _processor.DeleteDatabaseFile();
		}
	}

}
