using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;

namespace Kongo.Workers
{
	/// <summary>
	/// Process Node Statistics
	/// </summary>
	public class NodeStats : BackgroundService
	{
		private readonly ILogger<NodeStats> _logger;
		private readonly IProcessNodeStatistics _processor;
		private readonly NodeConfigurationModel _nodeConfiguration;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;
		private readonly KongoStatusModel _kongoStatus;

		public NodeStats(ILogger<NodeStats> logger, NodeConfigurationModel nodeConfiguration, IProcessNodeStatistics processor, KongoOptions opts, KongoStatusModel kongoStatus)
		{
			_logger = logger;
			_nodeConfiguration = nodeConfiguration;
			_httpClient = new HttpClient();
			_processor = processor;
			_sb = new StringBuilder();
			_opts = opts;
			_kongoStatus = kongoStatus;
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
					var requestUri = new UriBuilder("http", host, port, "api/v0/node/stats");

					try
					{
						var response = await client.GetAsync(requestUri.Uri);

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

						var nodeStatistics = await _processor.ProcessNodeStatistics(content);

						_kongoStatus.PoolState = nodeStatistics.State;
						
						if (long.TryParse(nodeStatistics.LastBlockHeight, out long blockHeight))
							_kongoStatus.CurrentBlockHeight = blockHeight;
						else
							_kongoStatus.CurrentBlockHeight = 0;

						_kongoStatus.LastBlockReceivedAt = nodeStatistics.LastBlockTime.Value;
						_kongoStatus.PoolUptime = TimeSpan.FromSeconds(nodeStatistics.Uptime);

						_sb.Clear();
						_sb.AppendLine($"NodeStatistics running at: {DateTimeOffset.Now}");
						_sb.AppendLine();
						_sb.AppendLine($"BlockRecvCnt: {nodeStatistics.BlockRecvCnt}");
						_sb.AppendLine($"LastBlockDate: {nodeStatistics.LastBlockDate}");
						_sb.AppendLine($"LastBlockFees: {nodeStatistics.LastBlockFees}");
						_sb.AppendLine($"LastBlockHash: {nodeStatistics.LastBlockHash}");
						_sb.AppendLine($"LastBlockHeight: {nodeStatistics.LastBlockHeight}");
						_sb.AppendLine($"lastBlockSum: {nodeStatistics.lastBlockSum}");
						_sb.AppendLine($"LastBlockTime: {nodeStatistics.LastBlockTime}");
						_sb.AppendLine($"LastBlockTx: {nodeStatistics.LastBlockTx}");
						_sb.AppendLine($"TxRecvCnt: {nodeStatistics.TxRecvCnt}");
						_sb.AppendLine($"Uptime: {nodeStatistics.Uptime}");
						_sb.AppendLine();

						_logger.LogInformation(_sb.ToString());
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.Message);
						if (ex.InnerException != null)
							_logger.LogError(ex.InnerException, ex.InnerException.Message);
					}

				}

				await Task.Delay(30000, stoppingToken);
			}
		}
	}
}
