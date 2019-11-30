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
	public class Settings : BackgroundService
	{
		private readonly ILogger<Settings> _logger;
		private readonly IProcessSettings _processor;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public Settings(ILogger<Settings> logger, IProcessSettings processor, KongoOptions opts)
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

				var requestUri = new UriBuilder(uirScheme, host, port, "api/v0/settings");

				try
				{
					var response = await _httpClient.GetAsync(requestUri.Uri);
					string content = await response.Content.ReadAsStringAsync();

					if (_opts.Verbose || _opts.VerbosePoolSettings)
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

					var processedSettings = await _processor.ProcessSettings(content);

					_sb.Clear();
					_sb.AppendLine($"Settings running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
					_sb.AppendLine();
					_sb.AppendLine();
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"Last Updated".PadRight(20, ' ')} {processedSettings.Timestamp.ToString("g").PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"Block0Hash".PadRight(20, ' ')} {processedSettings.Block0Hash.PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"Block0Time".PadRight(20, ' ')} {processedSettings.Timestamp.ToString("g").PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"ConsensusVersion".PadRight(20, ' ')} {processedSettings.ConsensusVersion.PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"CurrSlotStartTime".PadRight(20, ' ')} {processedSettings.CurrSlotStartTime.ToString("g").PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"Fees".PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(5, ' ')} {"Certificate".PadRight(20, ' ')} {processedSettings.Certificate.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(5, ' ')} {"Coefficient".PadRight(20, ' ')} {processedSettings.Coefficient.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(5, ' ')} {"Constant".PadRight(20, ' ')} {processedSettings.Constant.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"MaxTxsPerBlock".PadRight(20, ' ')} {processedSettings.MaxTxsPerBlock.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"LastUpdated".PadRight(20, ' ')} {processedSettings.MaxTxsPerBlock.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"SlotDuration".PadRight(20, ' ')} {processedSettings.SlotDuration.ToString().PadRight(20, ' ')}");
					_sb.AppendLine($"{"".PadRight(2, ' ')} {"SlotsPerEpoch".PadRight(20, ' ')} {processedSettings.SlotsPerEpoch.ToString().PadRight(20, ' ')}");
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
