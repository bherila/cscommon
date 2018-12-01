using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Common {
	public static class CommonCache {
		public static CacheItemPolicy DefaultCachePolicy => new CacheItemPolicy {
			AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(3)
		};

		public static T AddCache<T>(string key, T value) {
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			if (value == null)
				return default(T); // can't cache null value
			var cache = MemoryCache.Default;
			if (cache.Contains(key) && cache[key] is T) {
				cache.Set(new CacheItem(key, value), DefaultCachePolicy);
			} else if (!cache.Contains(key) || !(cache[key] is T)) {
				cache?.Add(new CacheItem(key, value), DefaultCachePolicy);
				return value;
			}

			return (T) cache[key];
		}

		/// <summary>
		///     Returns or generates a memory-cached value
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="producer"></param>
		/// <returns></returns>
		public static T CachedValue<T>(string key, Func<T> producer) {
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
			var cache = MemoryCache.Default;
			if (cache == null || !cache.Contains(key) || !(cache[key] is T)) {
				var value = producer();
				if (value == null)
					return default(T); // can't cache null value
				cache?.Add(new CacheItem(key, value), DefaultCachePolicy);
				return value;
			}

			return (T) cache[key];
		}

		public static void Remove(string key) {
			MemoryCache.Default.Remove(key);
		}

		public static void RemoveAll(IEnumerable<string> keys) {
			foreach (var key in keys) Remove(key);
		}
	}
}
