using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
	public static class Collections
	{

		private static readonly Random rng = new Random();
		public static double[] Normalize<T>(this IEnumerable<T> input)
		{
			var arr = input.Select(s => Convert.ToDouble(s)).ToArray();
			var sum = arr[0];
			for (var i = 1; i < arr.Length; ++i)
				sum += arr[i];
			var res = new double[arr.Length];
			for (var i = 0; i < res.Length; ++i)
				res[i] = arr[i] / sum;
			return res;
		}

		public static bool IsEmpty<T>(T[] array)
		{
			return array == null || array.Length == 0;
		}

		public static T2 GetValueOrDefault<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
		{
			if (dict.ContainsKey(key))
				return dict[key];
			return default(T2);
		}

		public static T2 GetOrInit<T1, T2>(this Dictionary<T1, T2> dict, T1 key, Func<T2> initializer)
		{
			if (dict.ContainsKey(key))
				return dict[key];
			var @new = initializer();
			dict.Add(key, @new);
			return @new;
		}

		public static ISet<T> AsSet<T>(this IEnumerable<T> source)
		{
			if (source == null)
				return null;
			var set = new HashSet<T>();
			set.UnionWith(source);
			return set;
		}

		public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
		{
			if (source == null)
				return null;
			var set = new LinkedList<T>(source);
			return set;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1) {
				n--;
				var k = rng.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static string DistinctCsv(this IEnumerable<string> src)
		{
			return string.Join(",", src
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.OrderBy(x => x)
				.Distinct());
		}
	}
}
