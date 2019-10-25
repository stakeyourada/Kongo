using System;

namespace Kongo.Core.Models
{
	public class ProcessedNetworkStatisticsModel
	{
		public int Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public int TotalEstablishedConnections { get; set; }
		public DateTimeOffset? LastBlockReceivedAt { get; set; }
		public DateTimeOffset? LastFragmentReceivedAt { get; set; }
		public DateTimeOffset? LastGossipReceivedAt { get; set; }
		public int BlocksReceivedInPast30Min { get; set; }
		public int FragmentsReceivedInPast30Min { get; set; }
		public int GossipReceivedInPast30Min { get; set; }
	}
}
