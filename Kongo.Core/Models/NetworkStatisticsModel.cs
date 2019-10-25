using System;

namespace Kongo.Core.Models
{

	public class NetworkStatisticsModel
	{
		public DateTimeOffset EstablishedAt { get; set; }
		public DateTimeOffset? LastBlockReceived { get; set; }
		public DateTimeOffset? LastFragmentReceived { get; set; }
		public DateTimeOffset? LastGossipReceived { get; set; }
		public string NodeId { get; set; }
	}
}
