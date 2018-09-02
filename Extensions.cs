using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Common
{
	public static class Extensions
	{
		
		public static string CleanUrl(this string url) {
			url = Regex.Replace(url, "^https?://cdn.filepicker.io", "", RegexOptions.IgnoreCase);
			return url.Trim();
		}

		public static void AddError(this List<ValidationResult> list, string error) {
			list.Add(new ValidationResult(error));
		}

		public static void AddCombine<T1, T2>(this Dictionary<T1, List<T2>> dict, T1 key, T2 value) {
			if (dict.ContainsKey(key)) dict[key].Add(value);
			else dict.Add(key, new List<T2> {value});
		}

		public static void AddCombine<T1, T2>(this Dictionary<T1, HashSet<T2>> dict, T1 key, T2 value) {
			if (dict.ContainsKey(key)) dict[key].Add(value);
			else dict.Add(key, new HashSet<T2> {value});
		}

		public static string JoinWithSpace(this IEnumerable<string> items) {
			return string.Join(" ", items);
		}

		public static string JoinWithAnd(this IEnumerable<string> items) {
			var sb = new StringBuilder();
			var list = items.ToList();
			for (var i = 0; i < list.Count; ++i) {
				sb.Append(list[i]);
				if (i < list.Count - 2) sb.Append(", ");
				if (i == list.Count - 2) sb.Append(", and ");
			}
			return sb.ToString();
		}

		public static Guid AsGuid(this string guid) {
			Guid g;
			Guid.TryParse(guid, out g);
			return g;
		}

		public static T2 GetValueOrDefault<T1, T2>(this Dictionary<T1, T2> dict, T1 key) {
			if (dict.ContainsKey(key))
				return dict[key];
			return default(T2);
		}

		public static string SanitizeEmail(this string email) {
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
	}
}
