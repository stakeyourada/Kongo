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
	/// Process Blockchain Fragments
	/// </summary>
	public class Fragments : BackgroundService
	{
		private readonly ILogger<Fragments> _logger;
		private readonly IProcessFragments _processor;
		private readonly NodeConfigurationModel _nodeConfiguration;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;

		public Fragments(ILogger<Fragments> logger, NodeConfigurationModel nodeConfiguration, IProcessFragments processor)
		{
			_logger = logger;
			_nodeConfiguration = nodeConfiguration;
			_httpClient = new HttpClient();
			_processor = processor;
			_sb = new StringBuilder();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var portPart = _nodeConfiguration.Rest.Listen.Substring(_nodeConfiguration.Rest.Listen.IndexOf(':') + 1, _nodeConfiguration.Rest.Listen.Length - (_nodeConfiguration.Rest.Listen.IndexOf(':') + 1));
				var host = _nodeConfiguration.Rest.Listen.Substring(0, _nodeConfiguration.Rest.Listen.IndexOf(':'));
				Int32.TryParse(portPart, out int port);
				var requestUri = new UriBuilder("http", host, port, "api/v0/fragment/logs");

				try
				{
					var response = await _httpClient.GetAsync(requestUri.Uri);

					//will throw an exception if not successful
					response.EnsureSuccessStatusCode();

					string content = await response.Content.ReadAsStringAsync();
					//_logger.LogInformation(content);

					var processedFragments = await _processor.ProcessFragments(content);

					_sb.Clear();
					_sb.AppendLine($"Fragments running at: {DateTimeOffset.Now}");
					_sb.AppendLine();
					_sb.AppendLine($"Total Fragments: {processedFragments.TotalFragments}");
					_sb.AppendLine();
					_sb.AppendLine("Fragments from:");
					_sb.AppendLine($"\t   Rest: {processedFragments.FragmentsReceviedFromRest}");
					foreach (var frag in processedFragments.RestFragments)
					{
						_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
					}
					_sb.AppendLine($"\tNetwork: {processedFragments.FragmentsReceviedFromNetwork}");
					foreach (var frag in processedFragments.NetworkFragments)
					{
						_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
					}
					_sb.AppendLine();
					_sb.AppendLine("Fragments status:");
					_sb.AppendLine($"\tPending: {processedFragments.FragmentsPending}");
					foreach (var frag in processedFragments.PendingFragments)
					{
						_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
					}
					_sb.AppendLine($"\tInBlock: {processedFragments.FragmentsInBlock}");
					foreach (var frag in processedFragments.BlockFragments)
					{
						_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
					}
					_sb.AppendLine();

					_logger.LogInformation(_sb.ToString());
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);
				}

				await Task.Delay(30000, stoppingToken);
			}

			_httpClient.Dispose();
		}
	}
}
