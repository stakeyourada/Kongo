using System;
using System.Diagnostics.CodeAnalysis;

namespace Kongo.Core.Models
{
	public class FeesModel
	{
		public long Certificate { get; set; }
		public long Coefficient { get; set; }
		public long Constant { get; set; }
	}

	public class ProcessedSettingsModel : IComparable<StoredSettingsModel>
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

		public int CompareTo([AllowNull] StoredSettingsModel other)
		{
			int differences = 0;
			if (other != null)
			{
				if (!this.Block0Hash.Equals(other.Block0Hash)) differences++;
				if (!this.Block0Time.Equals(other.Block0Time)) differences++;
				if (!this.ConsensusVersion.Equals(other.ConsensusVersion)) differences++;
				if (!this.CurrSlotStartTime.Equals(other.CurrSlotStartTime)) differences++;
				if (!this.Fees.Certificate.Equals(other.Certificate)) differences++;
				if (!this.Fees.Coefficient.Equals(other.Coefficient)) differences++;
				if (!this.Fees.Constant.Equals(other.Constant)) differences++;
				if (!this.MaxTxsPerBlock.Equals(other.MaxTxsPerBlock)) differences++;
				if (!this.SlotDuration.Equals(other.SlotDuration)) differences++;
				if (!this.SlotsPerEpoch.Equals(other.SlotsPerEpoch)) differences++;
			}
			else
			{
				differences++;
			}
			return differences;
		}
	}
}
