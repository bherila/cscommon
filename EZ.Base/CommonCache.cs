using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Common
{
	public static class CommonCache
	{
		public static CacheItemPolicy DefaultCachePolicy
		{
			get
			{
				return new CacheItemPolicy
				{
					AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(3),
				};
			}
		}

		public static T AddCache<T>(string key, T value)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (value == null)
				return default(T); // can't cache null value
			var cache = MemoryCache.Default;
			if (cache.Contains(key) && (cache[key] is T))
			{
				cache.Set(new CacheItem(key, value), CommonCache.DefaultCachePolicy);
			}
			else if (!cache.Contains(key) || !(cache[key] is T))
			{
				cache?.Add(new CacheItem(key, value), CommonCache.DefaultCachePolicy);
				return value;
			}

			return (T) cache[key];
		}

		public static void Remove(string key)
		{
			MemoryCache.Default.Remove(key);
		}

		public static void RemoveAll(IEnumerable<string> keys)
		{
			foreach (var key in keys)
			{
				Remove(key);
			}
		}
	}
}