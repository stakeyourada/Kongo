using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kongo.Core.Models
{
	[DataContract(Name = "NodeConfigurationFormats")]
	public enum NcFormats
	{
		[EnumMember(Value = "plain")]
		plain,

		[EnumMember(Value = "json")]
		json
	}

	[DataContract(Name = "NodeConfigurationLevels")]
	public enum NcLevels
	{
		[EnumMember(Value = "plain")]
		debug,

		[EnumMember(Value = "info")]
		info,

		[EnumMember(Value = "warning")]
		warning,

		[EnumMember(Value = "error")]
		error
	}

	public enum NcOutputs
	{
		[EnumMember(Value = "stdout")]
		stdout,

		[EnumMember(Value = "stderr")]
		stderr
	}

	public enum InterestLevels
	{
		[EnumMember(Value = "high")]
		high,

		[EnumMember(Value = "low")]
		low
	}

	public class NcLog
	{
		public NcLog ShallowCopy()
		{
			return this != null ? (NcLog)this.MemberwiseClone() : null;
		}

		public NcFormats Format { get; set; }
		public NcLevels Level { get; set; }
		public NcOutputs Output { get; set; }
	}

	public class TrustedPeer
	{
		public TrustedPeer ShallowCopy()
		{
			return this != null ? (TrustedPeer)this.MemberwiseClone() : null;
		}

		public string Address { get; set; }
		public string Id { get; set; }
	}

	public class NcTopicsOfInterest
	{
		public NcTopicsOfInterest ShallowCopy()
		{
			return this != null ? (NcTopicsOfInterest)this.MemberwiseClone() : null;
		}

		public InterestLevels Blocks { get; set; }
		public InterestLevels Messages { get; set; }
	}

	public class NcP2p
	{
		public NcP2p DeepCopy()
		{
			NcP2p other = (NcP2p)this.MemberwiseClone();
			other.Trusted_Peers = Trusted_Peers;
			return other; 
		}

		public string Private_Id { get; set; }
		public string Listen_Address { get; set; }
		public string Public_Address { get; set; }
		public List<TrustedPeer> Trusted_Peers { get; set; }
		public NcTopicsOfInterest Topics_of_interest { get; set; }
	}

	public class NcExplorer
	{
		public NcExplorer ShallowCopy()
		{
			return this != null ? (NcExplorer)this.MemberwiseClone() : null;
		}

		public bool Enabled { get; set; }
	}

	public class NcRest
	{
		public NcRest ShallowCopy()
		{
			return this != null ? (NcRest)this.MemberwiseClone() : null;
		}

		public string Listen { get; set; }
	}

	public class NodeConfigurationModel
	{
		public NodeConfigurationModel DeepCopy()
		{
			NodeConfigurationModel other = (NodeConfigurationModel)this.MemberwiseClone();
			other.Log = Log.ShallowCopy();
			other.P2p = P2p.DeepCopy();
			other.Rest = Rest.ShallowCopy();
			other.Explorer = Explorer.ShallowCopy();
			return other;
		}

		public NcLog Log { get; set; }
		public NcP2p P2p { get; set; }
		public NcExplorer Explorer { get; set; }
		public NcRest Rest { get; set; }
		public string Storage { get; set; }
	}

}
