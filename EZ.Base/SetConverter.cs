using System;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace Common {
	public class SetConverter<T> : CustomCreationConverter<ISet<T>> {
		public override ISet<T> Create(Type objectType) {
			return new HashSet<T>();
		}
	}
}