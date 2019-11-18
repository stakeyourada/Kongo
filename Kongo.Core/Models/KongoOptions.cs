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
		[Option('p', "pool-name", Required = true, HelpText = @"Stakepool Title name")]
		public string PoolName { get; set; }

		[Option('r', "rest-uri", Required = true, HelpText = @"Rest endpoint URI, e.g. http://127.0.0.1:3101")]
		public string RestUri { get; set; }

		[Option('d', "database-path", Required = true, HelpText = "path {folder path only} to Kongo.SQlite datbase")]
		public string DatabasePath { get; set; }

		[Option("pool-id", Required = false, HelpText = "Stakepool Id to track")]
		public string PoolId { get; set; }

		[Option("disable-all-collectors", Required = false, HelpText = "Disable all data collection workers")]
		public bool DisableDataCollection { get; set; }

		[Option("disable-node-stats", Required = false, HelpText = "Disable collection of node statistics every 30 seconds")]
		public bool NodeStats { get; set; }

		[Option("disable-network-stats", Required = false, HelpText = "Disable collection of network stats every 30 seconds")]
		public bool NetworkStats { get; set; }

		[Option("disable-fragment-logs", Required = false, HelpText = "Disable collection of fragment logs every 30 seconds")]
		public bool FragmentLogs { get; set; }

		[Option("show-fragments", Required = false, HelpText = "Show verbose list of fragment logs every 30 seconds, if fragment logs are being collected")]
		public bool ShowFragments { get; set; }

		[Option("disable-leader-logs", Required = false, HelpText = "Disable collection of leaders every 30 seconds")]
		public bool LeaderLogs { get; set; }

		[Option("disable-pool-settings", Required = false, HelpText = "Disable collection of pool settings every 30 seconds")]
		public bool PoolSettings { get; set; }

		[Option("disable-stake-pool-logs", Required = false, HelpText = "Disable collection of list of Stake Pools every 30 seconds")]
		public bool StakePools { get; set; }

		[Option("disable-stake-logs", Required = false, HelpText = "Disable collection of Stake Distribution logs every 30 seconds")]
		public bool StakeDistribution { get; set; }

		[Option("server.urls", Required = false, HelpText = "ASP.NET endpoint urls")]
		public string ServerUrls { get; set; }

		[Option('v', "verbose", Required = false, HelpText = "Verbose output")]
		public bool Verbose { get; set; }
		
		public DateTimeOffset ApplicationStartedOn { get; set; }

		[Usage(ApplicationAlias = "Kongo")]
		public static IEnumerable<Example> Examples =>
			new List<Example>() {
				new Example("\r\nSpecify name of Stakepool", new KongoOptions { PoolName = @"StakeYourAda.com" }),
				new Example("\r\nSpecify uri of Rest endpoint", new KongoOptions { RestUri = @"http://127.0.0.1:3101" }),
				new Example("\r\nSpecify stakepool nodeid", new KongoOptions { PoolId = @"8f779ef637831eb2acea6b3f9b3dbe4feb6e1d4ff49a06ef8bbec0d93a16db14" }),
				new Example("\r\nSpecify folder path to Kongo.SQlite datbase", new KongoOptions { DatabasePath = @".\" }),
				new Example("\r\nOutput verbose fragment logs", new KongoOptions { ShowFragments = true }),
				new Example("\r\nDisable all data collection workers", new KongoOptions { DisableDataCollection = true }),
				new Example("\r\nDisable collection of node statistics", new KongoOptions { NodeStats = true }),
				new Example("\r\nDisable collection of network statistics", new KongoOptions { NetworkStats = true }),
				new Example("\r\nDisable collection of fragment logs", new KongoOptions { FragmentLogs = true }),
				new Example("\r\nDisable collection of leaders log", new KongoOptions { LeaderLogs = true }),
				new Example("\r\nDisable collection of Pools settings", new KongoOptions { PoolSettings = true }),
				new Example("\r\nDisable collection of Stake Pools list", new KongoOptions { StakePools = true }),
				new Example("\r\nDisable collection of Stake Distribution", new KongoOptions { StakeDistribution = true }),
				new Example("\r\nASP.NET Core endpoint urls", new KongoOptions { ServerUrls = "http://localhost:5100;http://localhost:5101;http://*:5102" }),
				new Example("\r\nVerbose output, including raw rest response", new KongoOptions { Verbose = true })
			};

	}
}
