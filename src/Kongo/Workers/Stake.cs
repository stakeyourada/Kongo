using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Kongo.Workers
{
	/// <summary>
	/// Process Blockchain Fragments
	/// </summary>
	public class Stake : BackgroundService
	{
		private readonly ILogger<Stake> _logger;
		private readonly IProcessStake _processor;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public Stake(ILogger<Stake> logger, IProcessStake processor, KongoOptions opts)
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
				try
				{
					var uirScheme = _opts.RestUri.Split(':')[0];
					var host = _opts.RestUri.Split(':')[1].Substring(2);
					var portPart = _opts.RestUri.Split(':')[2];
					Int32.TryParse(portPart, out int port);

					var requestUri = new UriBuilder(uirScheme, host, port, "api/v0/stake");

					try
					{
						var response = await _httpClient.GetAsync(requestUri.Uri);
						string content = await response.Content.ReadAsStringAsync();

						if (_opts.Verbose || _opts.StakeDistribution)
						{
							var currentForeground = Console.ForegroundColor;
							Console.ForegroundColor = ConsoleColor.Cyan;
							Console.WriteLine(requestUri.Uri.ToString());
							Console.WriteLine(response);
							Console.WriteLine(content);
							Console.WriteLine();
							Console.ForegroundColor = currentForeground;
						}
						_sb.Clear();
						_sb.AppendLine($"Stake running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
						_sb.AppendLine();
						_sb.AppendLine();

						//will throw an exception if not successful
						response.EnsureSuccessStatusCode();

						var processedStake = await _processor.ProcessStake(content);

						var poolDistribution = JsonConvert.DeserializeObject<List<PoolDistribution>>(processedStake.PoolDistributionJson);

						long totalStaked = 0;
						long totalPools = 0;
						foreach (var pool in poolDistribution.OrderByDescending(p => p.AdaStaked))
						{
							totalStaked += pool.AdaStaked;
							totalPools++;
						}

						_sb.AppendLine($"{"".PadLeft(6, ' ')} {"Total # of Pools:".PadRight(30, ' ')} {totalPools.ToString().PadLeft(20, ' ')}");
						_sb.AppendLine($"{"".PadLeft(6, ' ')} {"Total Amount Staked:".PadRight(30, ' ')} {Convert.ToDecimal(totalStaked / 1000000).ToString("#,##0.000").PadLeft(20, ' ')}");
						_sb.AppendLine($"{"".PadLeft(6, ' ')} {"Total (staked + unstaked):".PadRight(30, ' ')} {Convert.ToDecimal((totalStaked + processedStake.Unassigned) / 1000000).ToString("#,##0.000").PadLeft(20, ' ')}");
						_sb.AppendLine();
						_sb.AppendLine($"{"".PadLeft(6, ' ')} {"Rank".PadRight(5, ' ')} {"Pool Id".PadRight(64, ' ')} {"Amount Staked".PadLeft(20, ' ')} {"% of Staked".PadLeft(11, ' ')} {"% of Total".PadLeft(11, ' ')} ");
						_sb.AppendLine($"{"".PadLeft(6, ' ')} {"".PadRight(5, '-')} {"".PadRight(64, '-')} {"".PadLeft(20, '-')} {"".PadLeft(11, '-')} {"".PadLeft(11, '-')} ");

						int top = 0;
						int rank = 0;
						foreach (var pool in poolDistribution.OrderByDescending(p => p.AdaStaked))
						{
							rank++;
							if (top++ < 50)
							{
								var percentStaked = ($"{(Convert.ToDecimal(pool.AdaStaked) / Convert.ToDecimal(totalStaked) * 100).ToString("##0.000")} %");
								var percentTotal = ($"{(Convert.ToDecimal(pool.AdaStaked) / Convert.ToDecimal(totalStaked + processedStake.Unassigned) * 100).ToString("##0.000")} %");

								if (pool.PoolId.Equals(_opts.PoolId))
								{
									_sb.AppendLine($"{"****".PadLeft(6, ' ')} {rank.ToString().PadRight(5, ' ')} {pool.PoolId.PadRight(64, ' ')} {Convert.ToDecimal(pool.AdaStaked / 1000000).ToString("#,##0.000").PadLeft(20, ' ')} {percentStaked.PadLeft(11, ' ')} {percentTotal.PadLeft(11, ' ')}");
								}
								else
								{
									_sb.AppendLine($"{"".PadLeft(6, ' ')} {rank.ToString().PadRight(5, ' ')} {pool.PoolId.PadRight(64, ' ')} {Convert.ToDecimal(pool.AdaStaked / 1000000).ToString("#,##0.000").PadLeft(20, ' ')} {percentStaked.PadLeft(11, ' ')} {percentTotal.PadLeft(11, ' ')}");
								}
							}
						}
						if (totalPools > 50)
						{
							_sb.AppendLine();
							_sb.AppendLine($"...{totalPools - 50} more not shown");
						}
						_sb.AppendLine();
						_logger.LogInformation(_sb.ToString());
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.GetBaseException(), ex.GetBaseException().Message);
					}
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
