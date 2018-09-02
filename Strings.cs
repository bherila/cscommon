using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Common
{
	public static class Strings
	{
        // \p{Mn} or \p{Non_Spacing_Mark}: 
		//   a character intended to be combined with another 
		//   character without taking up extra space 
		//   (e.g. accents, umlauts, etc.). 
		private static readonly Regex nonSpacingMarkRegex =
			new Regex(@"\p{Mn}", RegexOptions.Compiled);

		private static readonly Random rng = new Random();

		public static string AsPhone(this string str)
		{
			if (str == null)
				return null;
			try {
				var digits = new Queue<char>(str.Where(char.IsDigit));
				if (digits.Count == 11)
					digits.Dequeue();
				var sb = new StringBuilder();
				sb.Append('(').Append(digits.Dequeue()).Append(digits.Dequeue()).Append(digits.Dequeue()).Append(") ");
				sb.Append(digits.Dequeue()).Append(digits.Dequeue()).Append(digits.Dequeue()).Append("-");
				sb.Append(digits.Dequeue()).Append(digits.Dequeue()).Append(digits.Dequeue()).Append(digits.Dequeue());
				return sb.ToString();
			} catch {
				return str;
			}
		}

		public static string CleanUrl(this string url)
		{
			url = Regex.Replace(url, "^https?://cdn.filepicker.io", "", RegexOptions.IgnoreCase);
			return url.Trim();
		}

		public static void AddError(this List<ValidationResult> list, string error)
		{
			list.Add(new ValidationResult(error));
		}

		public static void AddCombine<T1, T2>(this Dictionary<T1, List<T2>> dict, T1 key, T2 value)
		{
			if (dict.ContainsKey(key)) dict[key].Add(value);
			else dict.Add(key, new List<T2> { value });
		}

		public static void AddCombine<T1, T2>(this Dictionary<T1, HashSet<T2>> dict, T1 key, T2 value)
		{
			if (dict.ContainsKey(key)) dict[key].Add(value);
			else dict.Add(key, new HashSet<T2> { value });
		}

		public static string JoinWithSpace(this IEnumerable<string> items)
		{
			return string.Join(" ", items);
		}

		public static string JoinWithAnd(this IEnumerable<string> items)
		{
			var sb = new StringBuilder();
			var list = items.ToList();
			for (var i = 0; i < list.Count; ++i) {
				sb.Append(list[i]);
				if (i < list.Count - 2) sb.Append(", ");
				if (i == list.Count - 2) sb.Append(", and ");
			}
			return sb.ToString();
		}

		public static Guid AsGuid(this string guid)
		{
			Guid g;
			Guid.TryParse(guid, out g);
			return g;
		}

		public static string SanitizeEmail(this string email)
		{
			if (email == null)
				return null;

			email = email.ToLower().Trim();

			if (email.EndsWith(".con"))
				email = email.Substring(0, email.Length - 4) + ".com";

			email = email.Replace("mailto:", "");

			if (!new EmailAddressAttribute().IsValid(email)) {
				throw new ValidationException("Email address is not valid");
			}

			return email;
		}


		// https://stackoverflow.com/questions/331279/how-to-change-diacritic-characters-to-non-diacritic-ones/8885159#8885159
		public static string RemoveDiacritics(this string text)
		{
			if (text == null)
				return string.Empty;
			var normalizedText = text.Normalize(NormalizationForm.FormD);
			return nonSpacingMarkRegex.Replace(normalizedText, string.Empty);
		}

		public static int InsensitiveIndexOf(this string haystack, string needle)
		{
			var compareInfo = CultureInfo.InvariantCulture.CompareInfo;
			var options = CompareOptions.IgnoreCase |
						CompareOptions.IgnoreSymbols |
						CompareOptions.IgnoreNonSpace;
			return compareInfo.IndexOf(haystack, needle, options);
		}


		public static string Sha2(this string pass, Guid salt)
		{
			var sha2 = SHA256.Create();
			var hash = sha2.ComputeHash(Encoding.Default.GetBytes(salt.ToString("D").ToLower() + pass));
			return Convert.ToBase64String(hash);
		}

		public static string Md5Hex(this string pass)
		{
			var md5 = MD5.Create();
			var dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(pass));
			var sb = new StringBuilder();
			for (var i = 0; i < dataMd5.Length; i++)
				sb.AppendFormat("{0:x2}", dataMd5[i]);
			return sb.ToString();
		}

		public static Guid Md5Guid(this string pass)
		{
			var md5 = MD5.Create();
			var dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(pass));
			return new Guid(dataMd5);
		}

		public static string Base64(this byte[] data)
		{
			return Convert.ToBase64String(data);
		}

		public static Guid Coalesce(params Guid?[] items)
		{
			foreach (var item in items)
				if (item != null && item.Value != default(Guid))
					return item.Value;
			return Guid.Empty;
		}

		public static string Coalesce(params string[] items)
		{
			foreach (var item in items)
				if (!string.IsNullOrWhiteSpace(item))
					return item;
			return null;
		}

		public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if (dict.ContainsKey(key))
				return dict[key];
			return default(TValue);
		}

		public static string StripTags(this string str)
		{
			if (str == null)
				return null;
			return Regex.Replace(str, "<[^>]*>", string.Empty);
		}

		public static string StripNonNumeric(this string str)
		{
			if (str == null)
				return null;
			var sb = new StringBuilder(str.Length);
			foreach (var chr in str)
				if (char.IsDigit(chr))
					sb.Append(chr);
			return sb.ToString();
		}

		public static string StripNonAlpha(this string str)
		{
			if (str == null)
				return null;
			var sb = new StringBuilder(str.Length);
			foreach (var chr in str)
				if (char.IsLetter(chr))
					sb.Append(chr);
			return sb.ToString();
		}

		private static bool IsBad(char chr)
		{
			if (chr == '’' || chr == '`' || chr == '\'' || chr == '‘' || chr > 255)
				return true;
			return false;
		}

		public static string StripNonAlphaNumeric(this string str)
		{
			if (str == null)
				return null;
			var sb = new StringBuilder(str.Length);
			foreach (var chr in str)
				if (!IsBad(chr) && (char.IsLetter(chr) || char.IsNumber(chr)))
					sb.Append(chr);
			return sb.ToString();
		}

		public static string StripNonAlphaNumericDashDot(this string str)
		{
			if (str == null)
				return null;
			var sb = new StringBuilder(str.Length);
			foreach (var chr in str)
				if (!IsBad(chr) && (char.IsLetter(chr) || char.IsNumber(chr) || chr == '.' || chr == '-'))
					sb.Append(chr);
			return sb.ToString();
		}

		public static string StripNonAlphaNumericDashDotSpace(this string str)
		{
			if (str == null)
				return null;
			var sb = new StringBuilder(str.Length);
			foreach (var chr in str)
				if (!IsBad(chr) && (char.IsLetter(chr) || char.IsNumber(chr) || chr == '.' || chr == '-' || chr == ' '))
					sb.Append(chr);
			return sb.ToString();
		}

		public static DateTime ToMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}

		public static string TrimEnd2(this string str)
		{
			if (str == null)
				return null;

			Func<Regex, bool> strip = (Regex x) => {
				var ms = x.Matches(str);
				if (ms.Count == 0)
					return false;
				var m = ms[ms.Count - 1];
				if (str.Length == m.Length + m.Index) {
					str = str.Substring(0, m.Index);
					return true;
				}
				return false;
			};

			var r1 = new Regex(@"\s+");
			var r2 = new Regex(@"<br\s*>", RegexOptions.IgnoreCase);
			var r3 = new Regex(@"<p>\s*</p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

			while (strip(r1) || strip(r2) || strip(r3)) { }

			return str;
		}

		public static Guid ToGuid(this int value)
		{
			var bytes = new byte[16];
			BitConverter.GetBytes((long)value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		public static Guid? ToGuid(this int? value)
		{
			if (value == null)
				return null;
			return value.Value.ToGuid();
		}

		public static Guid ToGuid(this long value)
		{
			var bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		public static Guid? ToGuid(this long? value)
		{
			if (value == null)
				return null;
			return value.Value.ToGuid();
		}

		public static int ToInt(this Guid value)
		{
			var b = value.ToByteArray();
			var bint = BitConverter.ToInt64(b, 0);
			return (int)bint;
		}

		public static int ToIntOrDefault(this Guid value)
		{
			try {
				var b = value.ToByteArray();
				var bint = BitConverter.ToInt64(b, 0);
				return (int)bint;
			} catch {
				return default(int);
			}
		}

		public static long ToLong(this Guid value)
		{
			var b = value.ToByteArray();
			var bint = BitConverter.ToInt64(b, 0);
			return bint;
		}

		public static int? ToInt(this Guid? value)
		{
			if (value == null)
				return null;
			return value.Value.ToInt();
		}

		public static string MaxLength(this string s, int maxLength)
		{
			if (string.IsNullOrEmpty(s))
				return s;

			if (s.Length < maxLength)
				return s;

			return new string(s.Take(maxLength).ToArray());
		}
	}
}
