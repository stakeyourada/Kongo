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
	public class StakePools : BackgroundService
	{
		private readonly ILogger<StakePools> _logger;
		private readonly IProcessStakePools _processor;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public StakePools(ILogger<StakePools> logger, IProcessStakePools processor, KongoOptions opts)
		{
			_logger = logger;
			_httpClient = new HttpClient();
			_processor = processor;
			_sb = new StringBuilder();
			_opts = opts;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var uirScheme = _opts.RestUri.Split(':')[0];
				var host = _opts.RestUri.Split(':')[1].Substring(2);
				var portPart = _opts.RestUri.Split(':')[2];
				Int32.TryParse(portPart, out int port);

				var requestUri = new UriBuilder(uirScheme, host, port, "api/v0/stake_pools");

				try
				{
					var response = await _httpClient.GetAsync(requestUri.Uri);

					string content = await response.Content.ReadAsStringAsync();

					if (_opts.Verbose || _opts.StakePools)
					{
						var currentForeground = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine(requestUri.Uri.ToString());
						Console.WriteLine(response);
						Console.WriteLine(content);
						Console.WriteLine();
						Console.ForegroundColor = currentForeground;
					}

					//will throw an exception if not successful
					response.EnsureSuccessStatusCode();

					var processedStakePools = await _processor.ProcessStakePools(content);

					_sb.Clear();
					_sb.AppendLine($"StakePools running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
					_sb.AppendLine();
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
