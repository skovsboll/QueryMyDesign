using System;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;

namespace SkinnyDip {
	public static class StringExtensions {

		[Pure]
		public static bool HasValue(this string str) {
			return !string.IsNullOrEmpty(str);
		}

		[Pure]
		public static string ToHumanReadableName(this string str) {
			Contract.Requires(str != null);
			return Regex.Replace(str, "([A-Z][a-z])", " $1").Trim();
		}

		private static readonly Regex InvalidChars = new Regex(@"[^\w\d]", RegexOptions.Compiled);
		[Pure]
		public static string ToIdentifier(this string str) {
			return InvalidChars.Replace(str, "");
		}

		[Pure]
		public static bool IsLong(this string str) {
			long i;
			return long.TryParse(str, out i);			
		}

		[Pure]
		public static long AsLong(this string str) {
			Contract.Requires(str != null);
			Contract.Requires(str != "");

			long i;
			if (long.TryParse(str, out i))
				return i;

			throw new ArgumentException("String value '" + str + "' is not convertible to an longeger.");
		}

		[Pure]
		public static long? AsLongOrNull(this string str) {
			if (str == null) return default(long?);

			long i;
			if (long.TryParse(str, out i))
				return i;

			return default(long?);
		}

	}
}