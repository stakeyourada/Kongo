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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

				CreateHostBuilder(args).Build().Run();
			}
			catch (ArgumentNullException)
			{
				// just ignore this exception and let app close, cmd line help was already displayed
			}

		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)			
				.ConfigureServices((hostContext, services) =>
				{
					// Configuration
					//services.AddSingleton<NodeConfigurationModel>(_NodeConfig);
					services.AddSingleton<KongoOptions>(_opts);

					// Inject data processors
					services.AddSingleton<IProcessFragments, FragmentProcessor>();
					services.AddSingleton<IProcessNetworkStatistics, NetworkStatisticsProcessor>();
					services.AddSingleton<IProcessNodeStatistics, NodeStatisticsProcessor>();
					services.AddSingleton<IProcessLeaders, LeadersProcessor>();
					services.AddSingleton<IProcessStakePools, StakePoolsProcessor>();
					services.AddSingleton<IProcessStake, StakeProcessor>();

					// Inject shared models and other services
					services.AddSingleton<KongoStatusModel>(_kongoStatus);

					/* https://cmatskas.com/net-core-dependency-injection-with-constructor-parameters-2/ */
					services.AddTransient<KongoDataStorage>(s => new KongoDataStorage(_dbConnectionString));

					// can opt out of all data collection and just run as website
					if(!_opts.DisableDataCollection)
					{
						// configure data collection workers
						if (!_opts.NodeStats)
							services.AddHostedService<NodeStats>();
						if (!_opts.FragmentLogs)
							services.AddHostedService<Fragments>();
						if (!_opts.NetworkStats)
							services.AddHostedService<NetworkStats>();
						if (!string.IsNullOrEmpty(_opts.PoolId))
						{
							if (!_opts.LeaderLogs)
								services.AddHostedService<Leaders>();
						}
						if (!_opts.StakeDistribution)
							services.AddHostedService<Stake>();
						if (!_opts.StakePools)
							services.AddHostedService<StakePools>();
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
