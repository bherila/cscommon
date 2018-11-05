using System;
using System.Collections.Generic;

namespace Common
{
	public abstract class CohortAnalysis<T>
	{

		public List<T> Data { get; set; }

		public abstract string CohortGetter(T item);

		public abstract decimal ValueGetter(T item);

		public decimal PriorCutoffValue;

	}

	public class CohortRow<T>
	{

		public int N { get; set; }

		public string Label { get; set; }

		public List<T> Items { get; set; }
	}

}
