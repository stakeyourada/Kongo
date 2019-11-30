using Kongo.Core.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kongo.Shared
{
	public class TimeframeSelectorBase : ComponentBase
	{
		[Inject]
		protected KongoStatusModel StatusModel { get; set; } 

		[Parameter]
		public TimeRangeEnum DataWindowFlag { get; set; } = TimeRangeEnum.OneHour;

		[Parameter]
		public int DataWindow { get; set; } = (int) TimeRangeEnum.OneHour;

		[Parameter]
		public int GroupInterval { get; set; } = TimeRangeMap.GetGroupingIntervalFromTimeRange(TimeRangeEnum.OneHour);

		[Parameter]
		public EventCallback OnDataWindowChanged { get; set; }

		protected Task OnTimeRangeClicked(TimeRangeEnum range)
		{
			StatusModel.CurrentChartTimeframe = range;
			DataWindowFlag = range;
			DataWindow = (int)DataWindowFlag;
			GroupInterval = TimeRangeMap.GetGroupingIntervalFromTimeRange(range);
			//DataWindowChanged(DataWindow);
			//await HandleRedraw();
			
			return Task.FromResult(true);
		}
	}
}
