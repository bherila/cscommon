using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Common {
	public class ValidationErrorModel {
		public ValidationErrorModel() {
			ValidationErrors = new List<string>();
		}

		public ValidationErrorModel(IEnumerable<string> errors) {
			ValidationErrors = new List<string>();
			AddError(errors);
		}

		public List<string> ValidationErrors { get; set; }

		public List<string> ValidationWarnings { get; set; } 

		public bool IsValid {
			get { return ValidationErrors.Count == 0; }
		}

		public void AddError(string error) {
			ValidationErrors.Add(error);
		}

		public void AddError(IEnumerable<string> errors) {
			if (errors == null)
				return;
			foreach (var error in errors)
				if (!string.IsNullOrEmpty(error))
					ValidationErrors.Add(error);
		}

		public void AddError(ValidationException ex) {
			ValidationErrors.Add(ex.Message);
		}
	}
}