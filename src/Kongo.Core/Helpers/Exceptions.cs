using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mail;

namespace Kongo.Core.Helpers
{
	/// <summary>
	/// Exception helper methods.
	/// </summary>
	public static class Exceptions
	{
		/// <summary>
		/// Throw an ArgumentNullException if the specified parameter is null.
		/// </summary>
		/// <param name="parameter">Parameter value.</param>
		/// <param name="name">Parameter name.</param>
		/// 
		public static void ThrowIfNull(object parameter, string name)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		/// <summary>
		/// Throw an ArgumentException if the specified parameter is Guid.Empty.
		/// </summary>
		/// <param name="parameter">Parameter value.</param>
		/// <param name="name">Parameter name.</param>
		/// 
		public static void ThrowIfGuidEmpty(Guid parameter, string name)
		{
			if (parameter == Guid.Empty)
			{
				throw new ArgumentException("Expected non-empty Guid.", name);
			}
		}

		/// <summary>
		/// Throw an ArgumentException if the specified string is empty or null.
		/// </summary>
		/// <param name="value">String value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// 
		public static void ThrowIfEmpty(string value, string parameterName)
		{
			ThrowIfNull(value, parameterName);

			if (value.Length == 0)
			{
				throw new ArgumentException(
					"Expected non-empty string.",
					parameterName);
			}
		}

		/// <summary>
		/// Throw an ArgumentException if the specified string is empty or null.
		/// </summary>
		/// <param name="value">String value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// 
		public static void ThrowIfCollectionEmpty<T>(ICollection<T> value, string parameterName)
		{
			ThrowIfNull(value, parameterName);

			if (value.Count == 0)
			{
				throw new ArgumentException(
					"Expected non-empty collection.",
					parameterName);
			}
		}

		public static void ThrowIfEnumerableEmpty<T>(IEnumerable<T> value, string parameterName)
		{
			ThrowIfNull(value, parameterName);

			if (!value.Any())
			{
				throw new ArgumentException(
					"Expected non-empty enumerable.",
					parameterName);
			}
		}

		/// <summary>
		/// Throw an ArgumentException if the specified string is not empty.
		/// </summary>
		/// <param name="value">String value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfNotEmpty(string value, string parameterName)
		{
			if (value == null) return;
			if (value.Length != 0)
			{
				throw new ArgumentException(
					"Expected empty string.",
					parameterName);
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified integer is not in specified range (inclusive).
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="min">Minimum value (inclusive).</param>
		/// <param name="max">Maximum value (inclusive).</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfOutOfRange(int value, int min, int max, string parameterName)
		{
			if (value < min || value > max)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					$"Expected value between {min} and {max} inclusive.");
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified date is not in specified range (inclusive).
		/// </summary>
		/// <param name="value">Date value.</param>
		/// <param name="min">Minimum value (inclusive).</param>
		/// <param name="max">Maximum value (inclusive).</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfOutOfRange(DateTime value, DateTime min, DateTime max, string parameterName)
		{
			if (value < min || value > max)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					$"Expected value between {min} and {max} inclusive.");
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified date is not in specified range (inclusive).
		/// </summary>
		/// <param name="value">Date value.</param>
		/// <param name="min">Minimum value (inclusive).</param>
		/// <param name="max">Maximum value (inclusive).</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfOutOfRange(DateTimeOffset value, DateTimeOffset min, DateTimeOffset max, string parameterName)
		{
			if (value < min || value > max)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					$"Expected value between {min} and {max} inclusive.");
			}
		}
		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified value is not 
		/// defined in the passed enum.
		/// </summary>
		/// <param name="enumType">Type of the enumeration.</param>
		/// <param name="value">Value of the parameter.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// 
		[SuppressMessage("Microsoft.MSInternal", "CA1803:AvoidCostlyCallsWherePossible", Justification = "Unable to use alternate techniques because this method doesn't know the enum type at complie time.")]
		[Conditional("DEBUG")]
		public static void ThrowIfNotDefined(Type enumType, object value, string parameterName)
		{
			if (!enumType.IsSubclassOf(typeof(Enum)))
			{
				throw new InvalidOperationException(
					"Unable to perform IsDefined operation on non-enum types.");
			}

			if (!Enum.IsDefined(enumType, value))
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					"Value for this parameter must be defined by the enum.");
			}
		}

		/// <summary>
		/// Throw an InvalidOperationException if the specified type does not 
		/// implement the specified interface.
		/// </summary>
		/// <param name="type">Type to check.</param>
		/// <param name="interfaceInQuestion">Interface that must be implemented.</param>
		/// 
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "BASELINE: BackLog", MessageId = "")]
		public static void ThrowIfInterfaceNotImplemented(Type type, Type interfaceInQuestion)
		{
			Type[] interfaceTypes = type.GetInterfaces();

			foreach (Type interfaceType in interfaceTypes)
			{
				if (interfaceType == interfaceInQuestion)
				{
					return;
				}
			}

			throw new InvalidOperationException($"{type.Name} must implement the required interface: {interfaceInQuestion.Name}.");
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified integer is less than or equal to zero.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfNotPositive(int value, string parameterName)
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					"Value for this parameter must be greater than zero.");
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified long value is less than or equal to zero.
		/// </summary>
		/// <param name="value">Int64 value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfNotPositive(long value, string parameterName)
		{
			if (value <= 0)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					"Value for this parameter must be greater than zero.");
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified integer is less than zero.
		/// </summary>
		/// <param name="value">Integer value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfNegative(int value, string parameterName)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					"Value for this parameter must be equal or greater than zero.");
			}
		}

		/// <summary>
		/// Throw an ArgumentOutOfRangeException if the specified TimeSpan is less than zero.
		/// </summary>
		/// <param name="value">TimeSpan value.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		public static void ThrowIfNegative(TimeSpan value, string parameterName)
		{
			if (value < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException(
					parameterName,
					value,
					"Value for this parameter must be equal or greater than zero.");
			}
		}

		/// <summary>
		/// Throw an ArgumentNullException if the specified string parameter is null or empty string.
		/// </summary>
		/// <param name="parameter">Parameter value.</param>
		/// <param name="name">Parameter name.</param>
		/// 
		public static void ThrowIfNullOrEmpty(string parameter, string name)
		{
			ThrowIfNull(parameter, name);
			ThrowIfEmpty(parameter, name);
		}

		/// <summary>
		/// Throw a FormatException if the specified string parameter is an invalid email address or null or empty or contains whitespace.
		/// </summary>
		/// <param name="parameter">Parameter value.</param>
		/// <param name="name">Parameter name.</param>
		public static void ThrowIfInvalidEmailAddress(string parameter, string name)
		{
			ThrowIfNull(parameter, name);
			ThrowIfEmpty(parameter.Trim(), name);
			new MailAddress(parameter);
		}

		public static void ThrowIfNotJson(string parameter, string name)
		{
			ThrowIfNull(parameter, name);
			ThrowIfEmpty(parameter.Trim(), name);

			var strInput = parameter.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
				(strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
			{
				try
				{
					var obj = JToken.Parse(strInput);
					return;
				}
				catch (JsonReaderException jex)
				{
					throw new ArgumentException(
						$"Unable to parse Json value for this parameter. Contents={parameter}",
						name,
						jex);
				}
				catch (Exception ex) //some other exception
				{
					throw new ArgumentException(
						$"An unexpected exception occured while attempting to parse Json value for this parameter. Contents={parameter}",
						name,
						ex);
				}
			}
			else
			{
				throw new ArgumentException(
					$"Unexpected format or Invalid Json value for this parameter. Contents={parameter}",
					name);
			}
		}
	}
}
