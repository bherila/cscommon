using System;
using System.Collections.Generic;

namespace Common {
	public class CollectionWithDefaultItem<T> {
		public CollectionWithDefaultItem() {
			Items = new List<T>();
		}

		public List<T> Items { get; set; }

		public Guid DefaultItemId { get; set; }
	}
}