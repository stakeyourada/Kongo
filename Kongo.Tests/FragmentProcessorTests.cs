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

		const string _fragmentStream1 = "[{\"fragment_id\":\"08c7adcab9xccba8ab7a4828061363e7cb770ae24d1795f2895e37d1e6f5f1fa\",\"received_from\":\"Network\",\"received_at\":\"2019-10-21T15:28:55.877653200+00:00\",\"last_updated_at\":\"2019-10-21T15:32:16.895628100+00:00\",\"status\":{\"InABlock\":{\"date\":\"241.12866\",\"block\":\"dcc225d22937812fc54436e237020d699613f48fadf1170ae6f8a8b8e6e6707c\"}}},{\"fragment_id\":\"7d62d78013e288878ad7d1873279d0cd65da44350fe9b27d54602d73e70e0f8e\",\"received_from\":\"Network\",\"received_at\":\"2019-10-21T15:32:18.728159700+00:00\",\"last_updated_at\":\"2019-10-21T15:32:18.728159700+00:00\",\"status\":\"Pending\"}]";
		const string _fragmentStream2 = "[{\"fragment_id\":\"aba2ca0f237519c7b2cac2b20947802df03ec7d63215bd9229c54f6a20dd2d1a\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:11:30.929038300+00:00\",\"last_updated_at\":\"2019-10-20T17:11:30.929038300+00:00\",\"status\":\"Pending\"},{\"fragment_id\":\"cddae4b324dce6e4f629301a3878e90c313667993f8e7a260d8ccc35e3e6c4dc\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:39:14.115232300+00:00\",\"last_updated_at\":\"2019-10-20T17:39:14.115232300+00:00\",\"status\":\"Pending\"},{\"fragment_id\":\"b2819ee6470001df0642ddd2395d346e4b395f5b97d8ba0b7a622380b3876387\",\"received_from\":\"Network\",\"received_at\":\"2019-10-20T17:13:14.279917800+00:00\",\"last_updated_at\":\"2019-10-20T17:13:14.279917800+00:00\",\"status\":\"Pending\"}]";


		[Theory]		
		[InlineData(_fragmentStream1)]
		[InlineData(_fragmentStream2)]
		public async Task ProcessValidFragmentStream(string value)
		{
			var storage = new KongoDataStorage($"Data Source={Path.GetRandomFileName()}");
			_processor = new FragmentProcessor(storage);
			var fragments = await _processor.ProcessFragments(value);
			Assert.True(fragments != null, "fragements == null");
			Assert.True(fragments.FragmentsReceviedFromNetwork > 0, $"FragmentsReceviedFromNetwork = {fragments.FragmentsReceviedFromNetwork}");
			Assert.True(fragments.FragmentsReceviedFromRest == 0, $"FragmentsReceviedFromRest = {fragments.FragmentsReceviedFromRest}");
			if (value.Contains("08c7adcab95ccba8ab7a4828061363e7cb770ae24d1795f2895e37d1e6f5f1fa"))
			{
				Assert.True(fragments.FragmentsInBlock > 0, $"FragmentsInBlock = {fragments.FragmentsInBlock}");
				Assert.True(fragments.FragmentsPending > 0, $"PendingFragments = {fragments.FragmentsPending}");
			}
			else
			{
				Assert.True(fragments.FragmentsInBlock == 0, $"FragmentsInBlock = {fragments.FragmentsInBlock}");
				Assert.True(fragments.FragmentsPending > 0, $"PendingFragments = {fragments.FragmentsPending}");
			}
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
