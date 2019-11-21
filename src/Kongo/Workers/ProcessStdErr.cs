using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kongo.Workers
{
	/// <summary>
	/// Process Blockchain Fragments
	/// </summary>
	public class ProcessStdErr : BackgroundService
	{
		private readonly ILogger<ProcessStdErr> _logger;
		private readonly IProcessStdError _processor;
		private readonly KongoOptions _opts;
		private readonly FileSystemWatcher _watcher;
		private readonly LogIngestionConfigModel _logIngestionConfig;
		private readonly Stopwatch _stopwatch;
		private readonly StringBuilder _sb;

		//private readonly NodeConfigurationModel _nodeConfiguration;

		public ProcessStdErr(ILogger<ProcessStdErr> logger, LogIngestionConfigModel logIngestionConfig, IProcessStdError processor, KongoOptions opts)
		{
			_logger = logger;
			_processor = processor;
			_opts = opts;
			_watcher = new FileSystemWatcher();
			_logIngestionConfig = logIngestionConfig;
			_stopwatch = new Stopwatch();
			_sb = new StringBuilder();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var linesRead = 0;

			using Stream stream = File.Open(_logIngestionConfig.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using (StreamReader reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					var logLine = reader.ReadLine();
					await _processor.IngestLogEntry(logLine, linesRead++);
					_logger.LogDebug(logLine);
					
				}

				Stopwatch stopWatch = new Stopwatch();
				stopWatch.Start();

				// Get the elapsed time as a TimeSpan value.
				TimeSpan ts = stopWatch.Elapsed;

				while (stoppingToken.IsCancellationRequested)
				{
					if (!reader.EndOfStream)
					{
						var logLine = reader.ReadLine();
						await _processor.IngestLogEntry(logLine, linesRead++);
						_logger.LogDebug(logLine);
					}
					else
					{
						if (_stopwatch.Elapsed > TimeSpan.FromSeconds(30))
						{
							var processedLogModel = await _processor.ProcessIngestedLogs();

							_sb.Clear();
							_sb.AppendLine($"Log Summary on {_opts.PoolName}, at: {DateTimeOffset.Now}");
							_sb.AppendLine();

							_logger.LogInformation(_sb.ToString());
							_stopwatch.Restart();
						}
						else
						{
							await Task.Delay(1000, stoppingToken);
						}
					}
				}
				_stopwatch.Stop();
			}
		}
	}
}
