using System.ComponentModel.DataAnnotations;

namespace Common {
	public class RequestNotHandledException : ValidationException {
		public RequestNotHandledException() : base("The request was not handled by any code path") { }
	}
}