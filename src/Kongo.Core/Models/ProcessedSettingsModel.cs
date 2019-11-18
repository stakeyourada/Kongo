using System;

namespace Kongo.Core.Models
{
	public class FeesModel
	{
		public long Certificate { get; set; }
		public long Coefficient { get; set; }
		public long Constant { get; set; }
	}

	public class ProcessedSettingsModel
	{
		public long Id { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public string Block0Hash { get; set; }
		public DateTimeOffset Block0Time { get; set; }
		public string ConsensusVersion { get; set; }
		public DateTimeOffset CurrSlotStartTime { get; set; }
		public FeesModel Fees { get; set; }
		public int MaxTxsPerBlock { get; set; }
		public int SlotDuration { get; set; }
		public int SlotsPerEpoch { get; set; }
	}

}
