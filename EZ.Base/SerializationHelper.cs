using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EZ.Base {
	public static class SerializationHelper {
		public static bool TryJsonParse<T>(string json, ILogger logger, out T value) {
			try {
				if (!string.IsNullOrEmpty(json)) {
					value = JsonConvert.DeserializeObject<T>(json);
					if (value != null)
						return true;
				} else {
					logger?.LogWarning($"Warning thrown in {nameof(TryJsonParse)}: content is null or empty");
				}
			} catch (Exception ex) {
				// ignore
				logger?.LogWarning($"Exception thrown in {nameof(TryJsonParse)}: {ex.Message}");
			}

			value = default(T);
			return false;
		}
	}
}
