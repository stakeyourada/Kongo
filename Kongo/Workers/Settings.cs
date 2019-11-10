﻿using Microsoft.Extensions.Hosting;
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

					var processedStake = await _processor.ProcessSettings(content);

					_sb.Clear();
					_sb.AppendLine($"Settings running at: {DateTimeOffset.Now}");
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
