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
		[Option('r', "rest-uri", Required = true, HelpText = @"Rest endpoint URI, e.g. http://127.0.0.1:3101")]
		public string RestUri { get; set; }

		[Option('s', "node-secrets", Required = false, HelpText = "path to node secrets configuration")]
		public string NodeSecrets { get; set; }

		[Option('d', "database-path", Required = true, HelpText = "path {folder path only} to Kongo.SQlite datbase")]
		public string DatabasePath { get; set; }

		[Option("node-stats", Required = false, HelpText = "Refresh node stats every 30 seconds")]
		public bool NodeStats { get; set; }

		[Option("network-stats", Required = false, HelpText = "Refresh network stats every 30 seconds")]
		public bool NetworkStats { get; set; }

		[Option("fragment-logs", Required = false, HelpText = "Refresh fragment logs every 30 seconds")]
		public bool FragmentLogs { get; set; }

		[Option("show-fragments", Required = false, HelpText = "Output verbose list of fragment logs every 30 seconds, requires --fragment-logs")]
		public bool ShowFragments { get; set; }

		[Option("leader-logs", Required = false, HelpText = "Refresh leaders every 30 seconds")]
		public bool LeaderLogs { get; set; }

		[Option("stake-pool-logs", Required = false, HelpText = "Refresh list of Stake Pools every 30 seconds")]
		public bool StakePools { get; set; }

		[Option("stake-logs", Required = false, HelpText = "Refresh Stake Distribution logs every 30 seconds")]
		public bool StakeDistribution { get; set; }

		[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
		public bool Verbose { get; set; }

		public DateTimeOffset ApplicationStartedOn { get; set; }

		[Usage(ApplicationAlias = "Kongo")]
		public static IEnumerable<Example> Examples =>
			new List<Example>() {
				new Example("\r\nSpecify uri of Rest endpoint", new KongoOptions { RestUri = @"http://127.0.0.1:3101" }),
				new Example("\r\nSpecify path to node secrets configuration file", new KongoOptions { NodeSecrets = @".\node-secrets.yaml" }),
				new Example("\r\nSpecify folder path to Kongo.SQlite datbase", new KongoOptions { DatabasePath = @".\" }),
				new Example("\r\nMonitor node statistics", new KongoOptions { NodeStats = true }),
				new Example("\r\nMonitor network statistics", new KongoOptions { NetworkStats = true }),
				new Example("\r\nMonitor fragment logs", new KongoOptions { FragmentLogs = true }),
				new Example("\r\nOutput verbose fragment logs", new KongoOptions { ShowFragments = true }),
				new Example("\r\nMonitor leaders log", new KongoOptions { LeaderLogs = true }),
				new Example("\r\nMonitor Stake Pools list", new KongoOptions { StakePools = true }),
				new Example("\r\nMonitor Stake Distribution", new KongoOptions { StakeDistribution = true }),
				new Example("\r\nVerbose output", new KongoOptions { Verbose = true })
			};
	}
}
