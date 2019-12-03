namespace Kongo.Core.CustomExceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class KongoOptionsException : Exception
	{
		public KongoOptionsException()
		{
		}

		public KongoOptionsException(string message) : base(message)
		{
		}

		public KongoOptionsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected KongoOptionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
