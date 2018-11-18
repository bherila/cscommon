using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EZ.Base
{
	public class Stack
	{
		public string In { get; set; }
		public string At { get; set; }
	}

	public class ApiResult<T>
	{
		public T Value { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }

		public List<Stack> StackTrace { get; set; }

		public ApiResult<T> WithStack(string exStackTrace)
		{
			var rx = new Regex(@"at (.*?) in (.*)").Matches(exStackTrace);
			StackTrace = new List<Stack>();
			foreach (Match match in rx) StackTrace.Add(new Stack { At = match.Groups[2].Value, In = match.Groups[1].Value });

			return this;
		}
	}
}
