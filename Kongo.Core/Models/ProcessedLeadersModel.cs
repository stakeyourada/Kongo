﻿using System;
using System.Collections.Generic;

namespace Kongo.Core.Models
{

	public class ProcessedLeadersModel
	{
		public DateTimeOffset Timestamp { get; set; }
		public List<int> Leaders { get; set; }
	}
}
