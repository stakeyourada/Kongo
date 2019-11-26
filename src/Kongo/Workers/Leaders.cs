using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Expressions;

namespace Kongo.Workers
{
	/// <summary>
	/// Process Blockchain Fragments
	/// </summary>
	public class Leaders : BackgroundService
	{
		private readonly ILogger<Leaders> _logger;
		private readonly IProcessLeaders _processor;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public Leaders(ILogger<Leaders> logger, IProcessLeaders processor, KongoOptions opts)
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
				var requestUri = new UriBuilder(uirScheme, host, port, "api/v0/leaders");

				try
				{
					try
					{
						_httpClient.DefaultRequestHeaders
						  .Accept
						  .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header

						var response = await _httpClient.GetAsync(requestUri.Uri);

						var content = await response.Content.ReadAsStringAsync();

						if (_opts.Verbose || _opts.LeaderLogs)
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

						_sb.Clear();
						_sb.AppendLine($"Leaders Logs running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
						_sb.AppendLine();
						_sb.AppendLine();

						var processedLeaders = await _processor.ProcessLeaders(content);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, ex.Message);
					}

					_sb.AppendLine("\tChecking for Leader Logs....");
					try
					{
						requestUri = new UriBuilder(uirScheme, host, port, "api/v0/leaders/logs");
						_httpClient.DefaultRequestHeaders
								.Accept
								.Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header

						var response = await _httpClient.GetAsync(requestUri.Uri);
						var content = await response.Content.ReadAsStringAsync();

						if (_opts.Verbose || _opts.LeaderLogs)
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

						var processedLeaderLogs = await _processor.ProcessLeadersLogs(content);

						var leadersLogs = JsonConvert.DeserializeObject<List<StoredLeadersLogsModel>>(processedLeaderLogs.LeadersLogsJson);

						_sb.AppendLine();
						_sb.AppendLine();
						_sb.AppendLine($"{"Id".PadRight(2, ' ')} {"Created".PadRight(20, ' ')} {"Scheduled".PadRight(10, ' ')} {"Finished".PadRight(20, ' ')} {"Status".PadRight(8, ' ')} {"Block".PadRight(64, ' ')} {"Wake_at".PadRight(20, ' ')}");
						_sb.AppendLine($"{"".PadRight(2, '-')} {"".PadRight(20, '-')} {"".PadRight(10, '-')} {"".PadRight(20, '-')} {"".PadRight(8, '-')} {"".PadRight(64, '-')} {"".PadRight(20, '-')}");

						foreach (var log in leadersLogs.OrderBy(p => p.Scheduled_at_time))
						{
							var createdAtTime = log.Created_at_time.ToString("u");
							var finishedAt = log.Finished_at_time.HasValue ? log.Finished_at_time.Value.ToString("u") : "";
							var wakeAt = log.Wake_at_time.HasValue ? log.Wake_at_time.Value.ToString("u") : "";
							_sb.AppendLine($"{log.Enclave_leader_id.ToString().PadRight(2, ' ')} {createdAtTime.PadRight(20, ' ')} {log.Scheduled_at_date.PadRight(10, ' ')} {finishedAt.PadRight(20, ' ')} {log.Status.PadRight(8, ' ')} {(string.IsNullOrEmpty(log.Block) ? "" : log.Block).PadRight(64, ' ')} {wakeAt.PadRight(20, ' ')}");
						}

						_logger.LogInformation(_sb.ToString());
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, ex.Message);
						throw;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, ex.Message);
				}

				await Task.Delay(30000, stoppingToken);
			}

			_httpClient.Dispose();
		}

	}
}
