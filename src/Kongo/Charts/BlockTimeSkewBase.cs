using Blazorise.Charts;
using Kongo.Core.DataServices;
using Kongo.Core.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Kongo.Shared
{
	public class BlockTimeSkewBase : ComponentBase
	{
		private const long NSPerSecond = 10000000;

		private List<NodeStatisticsChartModel> _nodeStatisticsHistory;
		private List<string> _backgroundColors_BlockTimeSkew = new List<string> { ChartColor.FromRgba(255, 204, 153, 0.2f) };
		private List<string> _borderColors_BlockTimeSkew = new List<string> { ChartColor.FromRgba(255, 204, 153, 1f) };

		[Inject]
		protected KongoStatusModel StatusModel { get; set; }

		[Inject]
		protected KongoDataStorage Database { get; set; } 

		[Parameter]		
		public LineChart<double> TimestampAnalysisChart { get; set; }

		[Parameter]
		public int DataWindow { get; set; } = (int) TimeRangeEnum.OneHour;

		[Parameter]
		public int GroupInterval { get; set; } = TimeRangeMap.GetGroupingIntervalFromTimeRange(TimeRangeEnum.OneHour);

		protected override Task OnInitializedAsync()
		{
			TimestampAnalysisChart = new LineChart<double>();
			return Task.FromResult(true);
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await HandleRedraw();
			}
		}

		async Task HandleRedraw()
		{
			TimestampAnalysisChart.Clear();

			RefreshHistory();

			TimestampAnalysisChart.AddLabel(_nodeStatisticsHistory.Select(p => p.Timestamp.ToString("g", DateTimeFormatInfo.InvariantInfo)).ToArray());
			TimestampAnalysisChart.AddDataSet(GetTimestampSkewForLastBlockTime());

			await TimestampAnalysisChart.Update();
		}

		void RefreshHistory()
		{
			var allRecords = Database.NodeStatisticEntries.AsEnumerable();

			var filteredRecords = allRecords
									.Where(ns => ns.Timestamp > DateTimeOffset.UtcNow.AddMinutes(-DataWindow))
									.Select(t => new NodeStatisticsChartModel
									{
										Timestamp = t.Timestamp,
										LastBlockTime = t.LastBlockTime.HasValue ? t.LastBlockTime.Value : default,
										MaxBlockRecvCnt = t.BlockRecvCnt,
										MaxLastBlockHeight = long.Parse(t.LastBlockHeight),
										MaxUpTime = t.Uptime,
										MinUpTime = t.Uptime,
										SkewInMinutes = t.Timestamp.Subtract(t.LastBlockTime.HasValue ? t.LastBlockTime.Value : default).TotalMinutes
									}).ToList();

			var filteredRecordsCount = filteredRecords.Count();

			_nodeStatisticsHistory = filteredRecords.GroupBy(x =>
			{
				var stamp = x.Timestamp;
				stamp = stamp.AddMinutes(-(stamp.Minute % GroupInterval));
				stamp = stamp.AddMilliseconds(-stamp.Millisecond - 1000 * stamp.Second);
				stamp = stamp.AddTicks(-(stamp.UtcTicks % NSPerSecond));
				return stamp;
			})
			.Select(g => new NodeStatisticsChartModel
			{
				Timestamp = g.Key,
				SkewInMinutes = g.Average(s => s.SkewInMinutes),
				LastBlockTime = g.Max(s => s.LastBlockTime),
				MaxBlockRecvCnt = g.Max(s => s.MaxBlockRecvCnt),
				MaxLastBlockHeight = g.Max(s => s.MaxLastBlockHeight),
				MaxUpTime = g.Max(s => s.MaxUpTime),
				MinUpTime = g.Min(s => s.MinUpTime)
			}).ToList();

		}

		LineChartDataset<double> GetTimestampSkewForLastBlockTime()
		{
			return new LineChartDataset<double>
			{
				Label = "Block Time Skew (LastBlockTime vs Server Time) in minutes",
				Data = _nodeStatisticsHistory.Select(p => p.SkewInMinutes).ToList(),
				BackgroundColor = _backgroundColors_BlockTimeSkew,
				BorderColor = _borderColors_BlockTimeSkew,
				Fill = true,
				PointRadius = 2,
				BorderDash = new List<int> { }
			};
		}

	}
}
