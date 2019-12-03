using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using ElectronNET.API;
using Kongo.Core.CustomExceptions;
using Kongo.Core.DataProcessors;
using Kongo.Core.DataServices;
using Kongo.Core.Extensions;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using Kongo.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
				if (args.Any(a => a.Contains("help", StringComparison.InvariantCultureIgnoreCase)))
				{
					var parser = new CommandLine.Parser(with => with.HelpWriter = null);
					var result = parser.ParseArguments<KongoOptions>(args);
					var helpText = HelpText.AutoBuild(result, h =>
					{
						//configure HelpText
						h.AddPreOptionsLine("");
						h.AddPreOptionsLine("----------------------------------");
						h.AdditionalNewLineAfterOption = false; //remove newline between options
						h.Heading = "Kongo 1.0.2"; //change header
						h.Copyright = "Brought to you by Stakeyourada.com";
						return h;
					}, e => e);

					Console.WriteLine();
					Console.WriteLine(helpText);
				}
				else
				{
					// Parse command line and execute
					var config = new Program().ReadKongoConfiguration(args);

					CommandLine.Parser.Default.ParseArguments<KongoOptions>(config)
						.MapResult(
							(KongoOptions opts) => LoadKongoOptions(opts),
							HandleParseError);

					var host = CreateHostBuilder(args).Build();

					CreateDbIfNotExists(host);

					host.Run();
				}
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

				var logger = services.GetRequiredService<ILogger<Program>>();

				try
				{
					var context = services.GetRequiredService<KongoDataStorage>();
					context.Database.Migrate();

					using var connection = context.Database.GetDbConnection();
					logger.LogInformation($"Database migrated: {connection.ConnectionString}");
				}
				catch (Exception ex)
				{
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
					webBuilder.UseElectron(args);
					webBuilder.UseKestrel(options => options.ConfigureEndpoints());
					if (_opts != null && !string.IsNullOrEmpty(_opts.ServerUrls))
					{
						webBuilder.UseKestrel(options => options.ConfigureUrls(_opts.ServerUrls.Split(';', StringSplitOptions.RemoveEmptyEntries)));
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

			// parse database path and create db connection string
			try
			{
				if (File.Exists(_opts.DatabasePath) && !Directory.Exists(_opts.DatabasePath))
				{
					_dbConnectionString = $"Data Source={_opts.DatabasePath};";
				}
				else
				{
					if (_opts.DatabasePath.EndsWith(".sqlite", StringComparison.InvariantCultureIgnoreCase))
					{
						_dbConnectionString = $"Data Source={_opts.DatabasePath};";
					}
					else
					{
						if (!Directory.Exists(_opts.DatabasePath))
							Directory.CreateDirectory(_opts.DatabasePath);

						_dbConnectionString = $"Data Source={Path.Combine(_opts.DatabasePath, "Kongo.SQlite")};";
					}
				}
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

			// validate urls info
			if(!string.IsNullOrEmpty(_opts.ServerUrls))
			{
				if(string.IsNullOrEmpty(_opts.CertificateSubject))
				{
					if (string.IsNullOrEmpty(_opts.CertificatePath) || string.IsNullOrEmpty(_opts.CertificatePassword))
					{
						var message = "Missing dependent property:  (--server.url --cert-subject) or (--server.url --cert-path --cert-password) are required";
						Console.WriteLine(message);
						throw new KongoOptionsException(message);
					}
				}
			}

			// initialize Kongo Status model
			_kongoStatus = new KongoStatusModel()
			{
				CurrentBlockHeight = 0,
				CurrentChartTimeframe = TimeRangeEnum.OneHour,
				LastBlockReceivedAt = default,
				PoolState = "Jormungandr Initializing",
				PoolUptime = TimeSpan.FromSeconds(0)
			};

			return true;
		}

		public static object GetPropValue(object src, string propName)
		{
			return src.GetType().GetProperty(propName).GetValue(src, null);
		}

		private string[] ReadKongoConfiguration(string[] args)
		{
			var options = new Dictionary<string, string>();
			var results = new List<string>();

			// We store options in Kongo.options.json
			var defaultOptionsFile = $"{Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule?.FileName)}.options{".json"}";

			if (File.Exists(defaultOptionsFile))
			{
				// open and deserialize config
				var configurationJson = File.ReadAllText(defaultOptionsFile);
				var configuration = JsonConvert.DeserializeObject<KongoOptions>(configurationJson);

				// enumerate all the properties of our KongoOptions class and build dictionary of Option switches and configuration values from disk
				try
				{
					PropertyInfo[] argumentProperties = configuration.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
						.Where(p =>
							p.CustomAttributes.Any()
						).ToArray();

					foreach (PropertyInfo argumentProperty in argumentProperties)
					{
						var attribute = argumentProperty.CustomAttributes.Where(a => a.AttributeType == typeof(OptionAttribute)).FirstOrDefault();
						if (attribute != null)
						{
							var propertyValue = Convert.ChangeType(
								GetPropValue(configuration, argumentProperty.Name),
								argumentProperty.PropertyType);

							var arg = attribute.ConstructorArguments.Last();
							if (arg.ArgumentType.Equals(typeof(String)))
							{
								var longKey = arg.Value.ToString().Replace("\"", "");
								options.Add("--" + longKey, propertyValue == null ? "" : propertyValue.ToString());
							}
						}
					}
				}
				catch (Exception)
				{
				}

				// Update dictionary and override any settings passed in through the command line arguments
				for (int i = 0; i < args.GetUpperBound(0) + 1; i++)
				{
					if (options.ContainsKey(args[i]))
					{
						if (options.TryGetValue(args[i], out string configValue))
						{
							if (bool.TryParse(configValue, out bool isEnabled))
							{
								// if we were passed a bool switch, then set it to true
								if (!isEnabled)
								{
									options[args[i]] = true.ToString();
								}
							}
							else
							{
								// update with value from passed arguments
								options[args[i]] = args[i + 1];
							}
						}
					}
				}
			}

			Console.WriteLine("Current Settings:");
			Console.WriteLine();

			// build final list arguments after merge
			foreach (var option in options)
			{
				Console.WriteLine($"{option.Key} = {option.Value}");
				if (bool.TryParse(option.Value, out bool isEnabled))
				{
					if (isEnabled)
					{
						results.Add(option.Key);
					}
				}
				else
				{
					if (!string.IsNullOrEmpty(option.Value))
					{
						results.Add(option.Key);
						results.Add(option.Value);
					}
				}
			}
			Console.WriteLine();
			return results.ToArray();
		}
	}
}
