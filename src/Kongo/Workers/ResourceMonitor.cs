using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Kongo.Core.Interfaces;
using Kongo.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Kongo.Workers
{

	public class ResourceMonitor : BackgroundService
	{
		private readonly ILogger<ResourceMonitor> _logger;
		private readonly IProcessResourceUsage _processor;
		private readonly KongoOptions _opts;
		private readonly StringBuilder _sb;

		public ResourceMonitor(ILogger<ResourceMonitor> logger, IProcessResourceUsage processor, KongoOptions opts)
		{
			_logger = logger;
			_processor = processor;
			_opts = opts;
			_sb = new StringBuilder();
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var client = new HttpClient())
				{
					//var portPart = _nodeConfiguration.Rest.Listen.Substring(_nodeConfiguration.Rest.Listen.IndexOf(':') + 1, _nodeConfiguration.Rest.Listen.Length - (_nodeConfiguration.Rest.Listen.IndexOf(':') + 1));
					//var host = _nodeConfiguration.Rest.Listen.Substring(0, _nodeConfiguration.Rest.Listen.IndexOf(':'));
					//Int32.TryParse(portPart, out int port);
					//var requestUri = new UriBuilder("http", host, port, "api/v0/network/stats");

					try
					{
						int processid = 4444;
						//var cpuUsage = new CpuUsage();

						var currentProcess = Process.GetProcessById(processid);

						if (currentProcess == null)
						{
							continue;
						}
						var procName = currentProcess.ProcessName;

						/*
						 if (!this.allAppCpuData.Any(list => list.Id == id))
                    {
                        this.allAppCpuData.Add(new FabricResourceUsageData<int>("CPU Time", id));
                        this.allAppDiskReadsData.Add(new FabricResourceUsageData<float>("IO Read Bytes/sec", id));
                        this.allAppDiskWritesData.Add(new FabricResourceUsageData<float>("IO Write Bytes/sec", id));
                        this.allAppMemDataMB.Add(new FabricResourceUsageData<long>("Memory Consumption MB", id));
                        this.allAppMemDataPercent.Add(new FabricResourceUsageData<double>("Memory Consumption %", id));
                        this.allAppTotalActivePortsData.Add(new FabricResourceUsageData<int>("Total Active Ports", id));
                        this.allAppEphemeralPortsData.Add(new FabricResourceUsageData<int>("Ephemeral Ports", id));
                    }

// CPU (all cores)...
                    int i = Environment.ProcessorCount + 10;

                    while (!currentProcess.HasExited && i > 0)
                    {
                        this.Token.ThrowIfCancellationRequested();

                        int cpu = cpuUsage.GetCpuUsageProcess(currentProcess);

                        if (cpu >= 0)
                        {
                            this.allAppCpuData.FirstOrDefault(x => x.Id == id).Data.Add(cpu);
                        }

                        // Memory (private working set (process))...
                        var mem = this.perfCounters.PerfCounterGetProcessPrivateWorkingSetMB(currentProcess.ProcessName);
                        this.allAppMemDataMB.FirstOrDefault(x => x.Id == id).Data.Add((long)mem);

                        // Memory (percent in use (total))...
                        var memInfo = ObserverManager.TupleGetTotalPhysicalMemorySizeAndPercentInUse();
                        long totalMem = memInfo.Item1;

                        if (totalMem > -1)
                        {
                            double usedPct = Math.Round(((double)(mem * 100)) / (totalMem * 1024), 2);
                            this.allAppMemDataPercent.FirstOrDefault(x => x.Id == id).Data.Add(usedPct);
                        }

                        // Disk/Network/Etc... IO (per-process bytes read/write per sec)
                        this.allAppDiskReadsData.FirstOrDefault(x => x.Id == id)
                            .Data.Add(this.diskUsage.PerfCounterGetDiskIOInfo(
                                currentProcess.ProcessName,
                                "Process",
                                "IO Read Bytes/sec") / 1000);

                        this.allAppDiskWritesData.FirstOrDefault(x => x.Id == id)
                            .Data.Add(this.diskUsage.PerfCounterGetDiskIOInfo(
                                currentProcess.ProcessName,
                                "Process",
                                "IO Write Bytes/sec") / 1000);
                        --i;

                        Thread.Sleep(250);
                    }

                    // Total and Ephemeral ports....
                    this.allAppTotalActivePortsData.FirstOrDefault(x => x.Id == id)
                        .Data.Add(NetworkUsage.GetActivePortCount(currentProcess.Id));

                    this.allAppEphemeralPortsData.FirstOrDefault(x => x.Id == id)
                        .Data.Add(NetworkUsage.GetActiveEphemeralPortCount(currentProcess.Id));


						 */
						//var response = await _httpClient.GetAsync(requestUri.Uri);

						////will throw an exception if not successful
						//response.EnsureSuccessStatusCode();

						//string content = await response.Content.ReadAsStringAsync();
						//var processedNetworkStatistics = await _processor.ProcessNetworkStatistics(content);
						
						_sb.Clear();
						_sb.AppendLine($"ResourceMonitor running on {_opts.PoolName}, at: {DateTimeOffset.Now}");
						_sb.AppendLine();
						_sb.AppendLine();

						_logger.LogInformation(_sb.ToString());
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.Message);
					}
				}

				await Task.Delay(30000, stoppingToken);
			}
		}
	}
}
