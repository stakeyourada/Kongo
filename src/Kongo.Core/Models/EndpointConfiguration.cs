using System;
using System.Collections.Generic;
using System.Text;

namespace Kongo.Core.Models
{
	public class EndpointConfiguration
	{
		public string Host { get; set; }
		public int? Port { get; set; }
		public string Scheme { get; set; }
		public string StoreName { get; set; }
		public string StoreLocation { get; set; }
		public string FilePath { get; set; }
		public string Password { get; set; }
	}
}
