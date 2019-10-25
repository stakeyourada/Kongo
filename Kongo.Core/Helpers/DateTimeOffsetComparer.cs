using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Kongo.Core.Helpers
{
	public class DateTimeOffsetComparer<T> : IComparer<T>
	{
		private readonly PropertyInfo _property;

		public DateTimeOffsetComparer(string propertyName) => _property = typeof(T).GetProperty(propertyName);

		public int Compare(T left, T right)
		{
			return GetDateTimeOrDefault(left).CompareTo(GetDateTimeOrDefault(right));
		}

		private DateTimeOffset GetDateTimeOrDefault(T obj)
		{
			return
				DateTimeOffset.TryParse(_property.GetValue(obj) as string, out DateTimeOffset result)
				? result
				: default(DateTimeOffset);
		}
	}
}
