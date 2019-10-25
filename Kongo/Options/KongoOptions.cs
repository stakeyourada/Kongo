using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Kongo.Options
{
	/// <summary>
	/// Kongo Command Line Options Model
	/// </summary>
	public class KongoOptions
	{
		[Option('c', "node-config", Required = true, HelpText = "path to node configuration")]
		public string NodeConfig { get; set; }

		[Option('s', "node-secrets", Required = false, HelpText = "path to node secrets configuration")]
		public string NodeSecrets { get; set; }

		[Option("node-stats", Required = false, HelpText = "Refresh node stats every 30 seconds")]
		public bool NodeStats { get; set; }

		[Option("network-stats", Required = false, HelpText = "Refresh network stats every 30 seconds")]
		public bool NetworkStats { get; set; }

		[Option("fragment-logs", Required = false, HelpText = "Refresh fragment logs every 30 seconds")]
		public bool FragmentLogs { get; set; }

		[Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
		public bool Verbose { get; set; }

		[Usage(ApplicationAlias = "Kongo")]
		public static IEnumerable<Example> Examples =>
			new List<Example>() {
				new Example("\r\nSpecify path to node configuration file", new KongoOptions { NodeConfig = @".\node-config.yaml" }),
				new Example("\r\nSpecify path to node secrets configuration file", new KongoOptions { NodeConfig = @".\node-secrets.yaml" }),
				new Example("\r\nMonitor node statistics", new KongoOptions { NodeStats = true }),
				new Example("\r\nMonitor network statistics", new KongoOptions { NetworkStats = true }),
				new Example("\r\nMonitor fragment logs", new KongoOptions { FragmentLogs = true }),
				new Example("\r\nVerbose output", new KongoOptions { Verbose = true })
			};
	}
}
