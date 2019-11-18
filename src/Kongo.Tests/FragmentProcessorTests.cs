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
	public class FragmentProcessorTests
	{
		//private readonly ProfilesRepository _profilesRepository;
		//private readonly Mock<IProfileDataRepository> _moqProfileDataProvider;

		private IProcessFragments _processor;

		const string _fragmentStream1 = "[{\"fragment_id\":\"08c7adcab95ccba8ab7a4828061363e7cb770ae24d1795f2895e37d1e6f5f1fa\",\"received_from\":\"Network\",\"received_at\":\"2019-10-21T15:28:55.877653200+00:00\",\"last_updated_at\":\"2019-10-21T15:32:16.895628100+00:00\",\"status\":{\"InABlock\":{\"date\":\"241.12866\",\"block\":\"dcc225d22937812fc54436e237020d699613f48fadf1170ae6f8a8b8e6e6707c\"}}},{\"fragment_id\":\"7d62d78013e288878ad7d1873279d0cd65da44350fe9b27d54602d73e70e0f8e\",\"received_from\":\"Network\",\"received_at\":\"2019-10-21T15:32:18.728159700+00:00\",\"last_updated_at\":\"2019-10-21T15:32:18.728159700+00:00\",\"status\":\"Pending\"}]";
		const string _fragmentStream2 = "[{\"fragment_id\":\"aba2ca0f237519c7b2cac2b20947802df03ec7d63215bd9229c54f6a20dd2d1a\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:11:30.929038300+00:00\",\"last_updated_at\":\"2019-10-20T17:11:30.929038300+00:00\",\"status\":\"Pending\"},{\"fragment_id\":\"cddae4b324dce6e4f629301a3878e90c313667993f8e7a260d8ccc35e3e6c4dc\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:39:14.115232300+00:00\",\"last_updated_at\":\"2019-10-20T17:39:14.115232300+00:00\",\"status\":\"Pending\"},{\"fragment_id\":\"b2819ee6470001df0642ddd2395d346e4b395f5b97d8ba0b7a622380b3876387\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:13:14.279917800+00:00\",\"last_updated_at\":\"2019-10-20T17:13:14.279917800+00:00\",\"status\":\"Pending\"}]";
		const string _fragmentStream3 = "[{\"fragment_id\":\"db22d4501fe76e54a576fd24e5fdff5a46153629b8a720789133b9c9ee8ebbdb\",\"last_updated_at\":\"2019-11-14T14:31:49.734858900+00:00\",\"received_at\":\"2019-11-14T14:31:49.734858900+00:00\",\"received_from\":\"Network\",\"status\":\"Pending\"},{\"fragment_id\":\"1ce8f8b7d9177b66371073fa784dc90382442785d899bbc24b8f86309ef0353a\",\"last_updated_at\":\"2019-11-14T14:17:50.494119400+00:00\",\"received_at\":\"2019-11-14T14:17:40.344876300+00:00\",\"received_from\":\"Network\",\"status\":{\"InABlock\":{\"block\":\"76147c135d0017a4858b630979596b24da35bfd88579549e312b2b6ffdc4425d\",\"date\":\"39.6482\"}}},{\"fragment_id\":\"3ce898d11ace0feef2639715454d43c5826efc633804bf9cbf9d6d1ed0b01cc3\",\"last_updated_at\":\"2019-11-14T14:15:56.669685700+00:00\",\"received_at\":\"2019-11-14T14:02:06.904187+00:00\",\"received_from\":\"Network\",\"status\":{\"Rejected\":{\"reason\":\"Account with invalid signature\"}}}]";


		/*
const string _fragmentStream3 = "[{\"fragment_id\":\"db22d4501fe76e54a576fd24e5fdff5a46153629b8a720789133b9c9ee8ebbdb\",\"last_updated_at\":\"2019-11-14T14:31:49.734858900+00:00\",\"received_at\":\"2019-11-14T14:31:49.734858900+00:00\",\"received_from\":\"Network\",\"status\":\"Pending\"},{\"fragment_id\":\"1ce8f8b7d9177b66371073fa784dc90382442785d899bbc24b8f86309ef0353a\",\"last_updated_at\":\"2019-11-14T14:17:50.494119400+00:00\",\"received_at\":\"2019-11-14T14:17:40.344876300+00:00\",\"received_from\":\"Network\",\"status\":{\"InABlock\":{\"block\":\"76147c135d0017a4858b630979596b24da35bfd88579549e312b2b6ffdc4425d\",\"date\":\"39.6482\"}}},{\"fragment_id\":\"3ce898d11ace0feef2639715454d43c5826efc633804bf9cbf9d6d1ed0b01cc3\",\"last_updated_at\":\"2019-11-14T14:15:56.669685700+00:00\",\"received_at\":\"2019-11-14T14:02:06.904187+00:00\",\"received_from\":\"Network\",\"status\":{\"Rejected\":{\"reason\":\"Account with invalid signature\"}}}]";

			 */


		[Theory]
		[InlineData(_fragmentStream1)]
		[InlineData(_fragmentStream2)]
		[InlineData(_fragmentStream3)]
		public async Task ProcessValidFragmentStream(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new FragmentProcessor(storage);
			var fragments = await _processor.ProcessFragments(value);
			Assert.True(fragments != null, "fragements == null");
			Assert.True(fragments.TotalFragments == fragments.FragmentsInBlock + fragments.FragmentsRejected + fragments.FragmentsPending , $"Total Fragments = {fragments.TotalFragments}, All types added = {fragments.FragmentsInBlock + fragments.FragmentsRejected + fragments.FragmentsPending}");
		}

		[Theory]
		[InlineData(_fragmentStream3)]
		public async Task ParseAllValidStatusTypesFromFragmentStream(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new FragmentProcessor(storage);
			var fragments = await _processor.ProcessFragments(value);
			Assert.True(fragments != null, "fragements == null");
			Assert.True(fragments.FragmentsReceviedFromNetwork > 0, $"FragmentsReceviedFromNetwork = {fragments.FragmentsReceviedFromNetwork}");
			Assert.True(fragments.FragmentsReceviedFromRest == 0, $"FragmentsReceviedFromRest = {fragments.FragmentsReceviedFromRest}");
			Assert.True(fragments.FragmentsInBlock > 0, $"FragmentsInBlock = {fragments.FragmentsInBlock}");
			Assert.True(fragments.FragmentsRejected > 0, $"FragmentsRejected = {fragments.FragmentsRejected}");
			Assert.True(fragments.FragmentsPending > 0, $"PendingFragments = {fragments.FragmentsPending}");
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		[InlineData("this is not a json formatted string")]
		public void InvalidFragmentStream_Throws_ArgumentException(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new FragmentProcessor(storage);
			Assert.ThrowsAsync<ArgumentException>(() => _processor.ProcessFragments(value));
		}

	}
}
