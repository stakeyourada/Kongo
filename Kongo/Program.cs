using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Kongo.Options;
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

		public static void Main(string[] args)
		{

			// Parse command line and execute requested Verb
			CommandLine.Parser.Default.ParseArguments<KongoOptions>(args)
				.MapResult(
					(KongoOptions opts) => LoadKongoOptions(opts),
					HandleParseError);

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					// Configuration
					services.AddSingleton<NodeConfigurationModel>(_NodeConfig);

					// Inject data processors
					services.AddScoped<IProcessFragments, FragmentProcessor>();
					services.AddScoped<IProcessNetworkStatistics, NetworkStatisticsProcessor>();
					services.AddScoped<IProcessNodeStatistics, NodeStatisticsProcessor>();
					services.AddSingleton<SqliteConfigurationModel>(_sqliteConfiguration);
					services.AddTransient<KongoDataStorage>();

					// configure workers
					if (_opts.NodeStats)
						services.AddHostedService<NodeStats>();
					if (_opts.FragmentLogs)
						services.AddHostedService<Fragments>();
					if (_opts.NetworkStats)
						services.AddHostedService<NetworkStats>();
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

			_sqliteConfiguration = new SqliteConfigurationModel() { DatabaseName = "Kongo.SQlite" };

			return true;
		}
	}
}
