using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Kongo.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Kongo
{
	/// <summary>
	/// Kongō, A trident-shaped staff which emits a bright light in the darkness, and grants wisdom and insight. 
	/// The staff belonged originally to the Japanese mountain god Kōya-no-Myōjin.
	/// </summary>
	public class Program
	{
		private static KongoOptions _opts;
		private static KongoStatusModel _kongoStatus = null;
		private static string _dbConnectionString;

		public static void Main(string[] args)
		{
			try
			{
				// Parse command line and execute requested Verb
				CommandLine.Parser.Default.ParseArguments<KongoOptions>(args)
					.MapResult(
						(KongoOptions opts) => LoadKongoOptions(opts),
						HandleParseError);

				var host = CreateHostBuilder(args).Build();

				CreateDbIfNotExists(host);

				host.Run();
			}
			catch (ArgumentNullException)
			{
				// just ignore this exception and let app close, cmd line help was already displayed
			}
		}

		private static void CreateDbIfNotExists(IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;

				try
				{
					var context = services.GetRequiredService<KongoDataStorage>();
					context.Database.Migrate();
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred creating the DB.");
				}
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)			
				.ConfigureServices((hostContext, services) =>
				{
					// Configuration
					services.AddSingleton<KongoOptions>(_opts);

					// Inject data processors
					services.AddTransient<IProcessFragments, FragmentProcessor>();
					services.AddTransient<IProcessNetworkStatistics, NetworkStatisticsProcessor>();
					services.AddTransient<IProcessNodeStatistics, NodeStatisticsProcessor>();
					services.AddTransient<IProcessLeaders, LeadersProcessor>();
					services.AddTransient<IProcessStakePools, StakePoolsProcessor>();
					services.AddTransient<IProcessStake, StakeProcessor>();
					services.AddTransient<IProcessSettings, SettingsProcessor>();
					services.AddTransient<IRunDatabaseMaintenance, DatabaseMaintenance>();

					// Inject shared model and DbContext
					services.AddSingleton<KongoStatusModel>(s => _kongoStatus);
					services.AddTransient<KongoDataStorage>(s => new KongoDataStorage(_dbConnectionString));

					// can opt out of all data collection and just run as website
					if (!_opts.DisableDataCollection)
					{
						// configure data collection workers
						services.AddHostedService<NodeStats>();
						services.AddHostedService<Fragments>();
						services.AddHostedService<NetworkStats>();
						if (!string.IsNullOrEmpty(_opts.PoolId))
						{

							services.AddHostedService<Leaders>();
						}
						services.AddHostedService<Stake>();
						services.AddHostedService<StakePools>();
						services.AddHostedService<Settings>();
					}

					services.AddHostedService<Maintenance>();
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					if(_opts != null && !string.IsNullOrEmpty(_opts.ServerUrls))
					{
						webBuilder.UseUrls(_opts.ServerUrls.Split(';', StringSplitOptions.RemoveEmptyEntries));
					}					
					webBuilder.UseStartup<Startup>();
				});

		private static object HandleParseError(IEnumerable<Error> arg)
		{
			return null;
		}

		private static object LoadKongoOptions(KongoOptions opts)
		{
			_opts = opts;
			_opts.ApplicationStartedOn = DateTimeOffset.UtcNow;

			YamlDotNet.Serialization.Deserializer deserializer = new Deserializer();
			Newtonsoft.Json.JsonSerializer js = new JsonSerializer();

			// parse database path and create db connection string
			try
			{
				var databasePath = Path.GetDirectoryName(_opts.DatabasePath);
				if (databasePath == string.Empty)
				{
					databasePath = ".";
				}
				else
				{
					if (!Directory.Exists(databasePath))
						Directory.CreateDirectory(databasePath);
				}

				_dbConnectionString = $"Data Source={databasePath}/Kongo.SQlite;";
			}
			catch (Exception ex)
			{
				var currentForeground = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Exception occured parsing DatabasePath");
				Console.WriteLine($"Excaption: {ex}, {ex.Message}");
				Console.ForegroundColor = currentForeground;

				_dbConnectionString = "Data Source=Kongo.SQlite;";
			}

			// initialize Kongo Status model
			_kongoStatus = new KongoStatusModel()
			{
				CurrentBlockHeight = 0,
				CurrentChartTimeframe = TimeRangeEnum.OneHour,
				LastBlockReceivedAt = default,
				PoolState = "Kongo Initializing",
				PoolUptime = TimeSpan.FromSeconds(0)
			};

			return true;
		}
	}
}
