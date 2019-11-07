using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kongo.Workers
{
	/// <summary>
	/// Process Network Statistics
	/// </summary>
	public class NetworkStats : BackgroundService
	{
		private readonly ILogger<NetworkStats> _logger;
		private readonly IProcessNetworkStatistics _processor;
		private readonly NodeConfigurationModel _nodeConfiguration;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public NetworkStats(ILogger<NetworkStats> logger, NodeConfigurationModel nodeConfiguration, IProcessNetworkStatistics processor, KongoOptions opts)
		{
			_logger = logger;
			_nodeConfiguration = nodeConfiguration;
			_httpClient = new HttpClient();
			_processor = processor;
			_sb = new StringBuilder();
			_opts = opts;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var client = new HttpClient())
				{
					var portPart = _nodeConfiguration.Rest.Listen.Substring(_nodeConfiguration.Rest.Listen.IndexOf(':') + 1, _nodeConfiguration.Rest.Listen.Length - (_nodeConfiguration.Rest.Listen.IndexOf(':') + 1));
					var host = _nodeConfiguration.Rest.Listen.Substring(0, _nodeConfiguration.Rest.Listen.IndexOf(':'));
					Int32.TryParse(portPart, out int port);
					var requestUri = new UriBuilder("http", host, port, "api/v0/network/stats");

					try
					{
						var response = await _httpClient.GetAsync(requestUri.Uri);

						//will throw an exception if not successful
						response.EnsureSuccessStatusCode();

						string content = await response.Content.ReadAsStringAsync();

						if (_opts.Verbose)
						{
							var currentForeground = Console.ForegroundColor;
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.WriteLine(requestUri.Uri.ToString());
							Console.WriteLine(response);
							Console.WriteLine(content);
							Console.WriteLine();
							Console.ForegroundColor = currentForeground;
						}

						var processedNetworkStatistics = await _processor.ProcessNetworkStatistics(content);

						_sb.Clear();
						_sb.AppendLine($"NetworkStatistics running at: {DateTimeOffset.Now}");
						_sb.AppendLine();
						_sb.AppendLine($"Total Established Connections: {processedNetworkStatistics.TotalEstablishedConnections}");
						_sb.AppendLine();
						_sb.AppendLine("Last Events Timestamps:");
						_sb.AppendLine($"\t   Block Received: {processedNetworkStatistics.LastBlockReceivedAt}");
						_sb.AppendLine($"\tFragment Received: {processedNetworkStatistics.LastFragmentReceivedAt}");
						_sb.AppendLine($"\t  Gossip Received: {processedNetworkStatistics.LastGossipReceivedAt}");
						_sb.AppendLine();
						_sb.AppendLine("Past 30 Minutes stats:");
						_sb.AppendLine($"\t    Nodes Sending Block: {processedNetworkStatistics.BlocksReceivedInPast30Min}");
						_sb.AppendLine($"\tNodes Sending Fragments: {processedNetworkStatistics.FragmentsReceivedInPast30Min}");
						_sb.AppendLine($"\t        Nodes Gossiping: {processedNetworkStatistics.GossipReceivedInPast30Min}");
						_sb.AppendLine();

						_logger.LogInformation(_sb.ToString());
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.Message);
					}
				}

				await Task.Delay(30000, stoppingToken);
			}
		}
	}
}
