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

namespace Kongo.Workers
{
	/// <summary>
	/// Process Blockchain Fragments
	/// </summary>
	public class Fragments : BackgroundService
	{
		private readonly ILogger<Fragments> _logger;
		private readonly IProcessFragments _processor;
		private readonly HttpClient _httpClient;
		private readonly StringBuilder _sb;
		private readonly KongoOptions _opts;

		public Fragments(ILogger<Fragments> logger, IProcessFragments processor, KongoOptions opts)
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
				var requestUri = new UriBuilder(uirScheme, host, port, "api/v0/fragment/logs");

				try
				{
					_httpClient.DefaultRequestHeaders
					  .Accept
					  .Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header

					var response = await _httpClient.GetAsync(requestUri.Uri);

					if (_opts.Verbose || _opts.VerboseFragmentLogs)
					{
						var currentForeground = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine(requestUri.Uri.ToString());
						//Console.WriteLine(response);
						Console.WriteLine();
						Console.ForegroundColor = currentForeground;
					}

					string content = await response.Content.ReadAsStringAsync();

					if (_opts.Verbose)
					{
						var currentForeground = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine(content);
						Console.WriteLine();
						Console.ForegroundColor = currentForeground;
					}

					//will throw an exception if not successful
					response.EnsureSuccessStatusCode();

					var processedFragments = await _processor.ProcessFragments(content);

					_sb.Clear();
					_sb.AppendLine($"Fragments running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
					_sb.AppendLine();
					_sb.AppendLine($"Total Fragments: {processedFragments.TotalFragments}");
					_sb.AppendLine();
					_sb.AppendLine("Fragments from:");
					_sb.AppendLine($"\t   Rest: {processedFragments.FragmentsReceviedFromRest}");
					if (_opts.ShowAllFragmentData)
					{
						foreach (var frag in processedFragments.RestFragments)
						{
							_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
						}
					}
					_sb.AppendLine($"\tNetwork: {processedFragments.FragmentsReceviedFromNetwork}");
					if (_opts.ShowAllFragmentData)
					{
						foreach (var frag in processedFragments.NetworkFragments)
						{
							_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
						}
					}
					_sb.AppendLine();
					_sb.AppendLine("Fragments status:");
					_sb.AppendLine($"\tPending: {processedFragments.FragmentsPending}");
					if (_opts.ShowAllFragmentData)
					{
						foreach (var frag in processedFragments.PendingFragments)
						{
							_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
						}
					}
					_sb.AppendLine($"\tInBlock: {processedFragments.FragmentsInBlock}");
					if (_opts.ShowAllFragmentData)
					{
						foreach (var frag in processedFragments.BlockFragments)
						{
							_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
						}
					}
					_sb.AppendLine($"\tRejected: {processedFragments.FragmentsRejected}");
					if (_opts.ShowAllFragmentData)
					{
						foreach (var frag in processedFragments.RejectedFragments)
						{
							_sb.AppendLine($"\t\t\t{frag.Fragment_id}\t @ {frag.Received_at}");
						}
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
