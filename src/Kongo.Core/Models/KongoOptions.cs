using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace Kongo.Core.Models
{
	/// <summary>
	/// Kongo Command Line Options Model
	/// </summary>
	public class KongoOptions
	{
		[Option("pool-name", Required = true, HelpText = @"Stakepool Title name")]
		public string PoolName { get; set; }

		[Option("rest-uri", Required = true, HelpText = @"Rest endpoint URI, e.g. http://127.0.0.1:3101")]
		public string RestUri { get; set; }

		[Option("database-path", Required = true, HelpText = "Storage path to Kongo.SQlite datbase")]
		public string DatabasePath { get; set; }

		[Option("pool-id", Required = false, HelpText = "Stakepool Id to track")]
		public string PoolId { get; set; }

		[Option("explorer-uri", Required = true, HelpText = @"Block explorer URI, e.g. https://explorer.nightly.jormungandr-testnet.iohkdev.io/")]
		public string BlockExplorerUri { get; set; }

		[Option("disable-all-collectors", Required = false, HelpText = "Disable all data collection workers")]
		public bool DisableDataCollection { get; set; }

		[Option("verbose-node-stats", Required = false, HelpText = "Show verbose request details for collection of node statistics every 30 seconds")]
		public bool VerboseNodeStats { get; set; }

		[Option("verbose-network-stats", Required = false, HelpText = "Show verbose request details for collection of network stats every 30 seconds")]
		public bool VerboseNetworkStats { get; set; }

		[Option("verbose-fragment-logs", Required = false, HelpText = "Show verbose request details for collection of fragment logs every 30 seconds")]
		public bool VerboseFragmentLogs { get; set; }

		[Option("show-all-fragments", Required = false, HelpText = "Show verbose list of fragment logs every 30 seconds, if fragment logs are being collected")]
		public bool ShowAllFragmentData { get; set; }

		[Option("verbose-leader-logs", Required = false, HelpText = "Show verbose request details for collection of leaders every 30 seconds")]
		public bool VerboseLeaderLogs { get; set; }

		[Option("verbose-pool-settings", Required = false, HelpText = "Show verbose request details for collection of pool settings every 30 seconds")]
		public bool VerbosePoolSettings { get; set; }

		[Option("verbose-stake-pool-logs", Required = false, HelpText = "Show verbose request details for collection of list of Stake Pools every 30 seconds")]
		public bool VerboseStakePools { get; set; }

		[Option("verbose-stake-logs", Required = false, HelpText = "Show verbose request details for collection of Stake Distribution logs every 30 seconds")]
		public bool VerboseStakeDistribution { get; set; }

		[Option("server.urls", Required = false, HelpText = "ASP.NET endpoint urls")]
		public string ServerUrls { get; set; }

		[Option("verbose", Required = false, HelpText = "Show all Verbose output")]
		public bool Verbose { get; set; }
		
		public DateTimeOffset ApplicationStartedOn { get; set; }

		public KongoOptions ShallowCopy()
		{
			return (KongoOptions)this.MemberwiseClone();
		}

		[Usage(ApplicationAlias = "Kongo")]
		public static IEnumerable<Example> Examples =>
			new List<Example>() {
				new Example("\r\nSpecify name of Stakepool", new KongoOptions { PoolName = @"StakeYourAda.com" }),
				new Example("\r\nSpecify uri of Rest endpoint", new KongoOptions { RestUri = @"http://127.0.0.1:3101" }),
				new Example("\r\nSpecify stakepool nodeid", new KongoOptions { PoolId = @"8f779ef637831eb2acea6b3f9b3dbe4feb6e1d4ff49a06ef8bbec0d93a16db14" }),
				new Example("\r\nSpecify folder path to Kongo.SQlite datbase", new KongoOptions { DatabasePath = @"D:/storage" }),
				new Example("\r\nOutput verbose fragment logs", new KongoOptions { ShowAllFragmentData = true }),
				new Example("\r\nSpecify uri of the block explorer", new KongoOptions { RestUri = @"https://explorer.nightly.jormungandr-testnet.iohkdev.io/" }),
				new Example("\r\nDisable all data collection workers", new KongoOptions { DisableDataCollection = true }),
				new Example("\r\nShow verbose request details for collection of node statistics", new KongoOptions { VerboseNodeStats = true }),
				new Example("\r\nShow verbose request details for collection of network statistics", new KongoOptions { VerboseNetworkStats = true }),
				new Example("\r\nShow verbose request details for collection of fragment logs", new KongoOptions { VerboseFragmentLogs = true }),
				new Example("\r\nShow verbose request details for collection of leaders log", new KongoOptions { VerboseLeaderLogs = true }),
				new Example("\r\nShow verbose request details for collection of Pools settings", new KongoOptions { VerbosePoolSettings = true }),
				new Example("\r\nShow verbose request details for collection of Stake Pools list", new KongoOptions { VerboseStakePools = true }),
				new Example("\r\nShow verbose request details for collection of Stake Distribution", new KongoOptions { VerboseStakeDistribution = true }),
				new Example("\r\nASP.NET Core endpoint urls", new KongoOptions { ServerUrls = "http://localhost:5100;http://localhost:5101;http://*:5102" }),
				new Example("\r\nVerbose output, including raw rest response", new KongoOptions { Verbose = true })
			};

	}
}
