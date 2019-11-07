using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Helpers;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Kongo.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;

namespace Kongo
{
	/// <summary>
	/// Kongō, A trident-shaped staff which emits a bright light in the darkness, and grants wisdom and insight. 
	/// The staff belonged originally to the Japanese mountain god Kōya-no-Myōjin.
	/// </summary>
	public class Program
	{
		private static NodeConfigurationModel _NodeConfig = null;
		private static JObject _NodeSecrets = null;
		private static KongoOptions _opts;
		private static SqliteConfigurationModel _sqliteConfiguration = null;
		private static KongoStatusModel _kongoStatus = null;

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
			}

		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					// Configuration
					services.AddSingleton<NodeConfigurationModel>(_NodeConfig);
					services.AddSingleton<SqliteConfigurationModel>(_sqliteConfiguration);
					services.AddSingleton<KongoOptions>(_opts);

					// Inject data processors
					services.AddScoped<IProcessFragments, FragmentProcessor>();
					services.AddScoped<IProcessNetworkStatistics, NetworkStatisticsProcessor>();
					services.AddScoped<IProcessNodeStatistics, NodeStatisticsProcessor>();
					services.AddScoped<IProcessLeaders, LeadersProcessor>();
					services.AddScoped<IProcessStakePools, StakePoolsProcessor>();
					services.AddScoped<IProcessStake, StakeProcessor>();

					// Inject shared models and other services
					services.AddSingleton<KongoStatusModel>(_kongoStatus);
					services.AddTransient<KongoDataStorage>();

					// configure workers
					if (_opts.NodeStats)
						services.AddHostedService<NodeStats>();
					if (_opts.FragmentLogs)
						services.AddHostedService<Fragments>();
					if (_opts.NetworkStats)
						services.AddHostedService<NetworkStats>();
					if (_opts.LeaderLogs)
						services.AddHostedService<Leaders>();
					if (_opts.StakeDistribution)
						services.AddHostedService<Stake>();
					if (_opts.StakePools)
						services.AddHostedService<StakePools>();

				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
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

			// convert YAML object to json we can work with
			using (var r = new StreamReader(opts.NodeConfig))
			{
				var yamlObject = deserializer.Deserialize(r);

				using var w = new StringWriter();
				js.Serialize(w, yamlObject);
				string jsonText = w.ToString();

				_NodeConfig = JsonConvert.DeserializeObject<NodeConfigurationModel>(jsonText);

				if (opts.Verbose)
				{
					var currentForeground = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine($"Loaded {opts.NodeConfig}");
					Console.WriteLine(JObject.Parse(jsonText).ToString());
					Console.ForegroundColor = currentForeground;
				}
			}

			if (!string.IsNullOrEmpty(opts.NodeSecrets))
			{
				// convert YAML object to json we can work with
				using (var r = new StreamReader(opts.NodeSecrets))
				{
					var yamlObject = deserializer.Deserialize(r);

					using var w = new StringWriter();
					js.Serialize(w, yamlObject);
					string jsonText = w.ToString();
					_NodeSecrets = JObject.Parse(jsonText);

					if (opts.Verbose)
					{
						var currentForeground = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine($"Loaded {opts.NodeSecrets}");
						Console.WriteLine(_NodeSecrets.ToString());
						Console.ForegroundColor = currentForeground;
					}
				}
			}

			// parse database path and create Sqlite configuration object
			try
			{
				var databasePath = Path.GetDirectoryName(_opts.DatabasePath);
				if (databasePath == string.Empty) {
					databasePath = ".";
				} 
				else 
				{ 
					if (!Directory.Exists(databasePath))				
						Directory.CreateDirectory(databasePath);
				}

				_sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = $"{databasePath}\\Kongo.SQlite" };
			}
			catch (Exception ex)
			{
				var currentForeground = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Exception occured parsing DatabasePath");
				Console.WriteLine($"Excaption: {ex}, {ex.Message}");
				Console.ForegroundColor = currentForeground;

				_sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = $"Kongo.SQlite" };				
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
