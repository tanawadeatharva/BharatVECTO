using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TUGraz.VectoCore.Utils
{
	public static class ValidationHelper
	{
		public static IList<ValidationResult> Validate<T>(this T entity)
		{
			var context = new ValidationContext(entity);
			var results = new List<ValidationResult>();
			Validator.TryValidateObject(entity, new ValidationContext(entity), results, true);

			foreach (
				var p in
					entity.GetType()
						.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
										BindingFlags.FlattenHierarchy)) {
				var attrs = p.GetCustomAttributes(typeof(ValidationAttribute)).Cast<ValidationAttribute>().ToList();
				if (attrs.Any()) {
					var val = p.GetValue(entity);
					context.DisplayName = p.Name;
					context.MemberName = p.Name;
					Validator.TryValidateValue(val, context, results, attrs);
				}
			}

			foreach (
				var f in
					entity.GetType()
						.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
									BindingFlags.FlattenHierarchy)) {
				var attrs = f.GetCustomAttributes(typeof(ValidationAttribute)).Cast<ValidationAttribute>().ToList();
				if (attrs.Any()) {
					var val = f.GetValue(entity);
					context.DisplayName = f.Name;
					context.MemberName = f.Name;
					Validator.TryValidateValue(val, context, results, attrs);
				}
			}

			return results;
		}

		public static bool IsValid<T>(this T entity)
		{
			return Validator.TryValidateObject(entity, new ValidationContext(entity), null, true);
		}
	}


	public class ValidateObjectAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null) {
				return ValidationResult.Success;
			}

			var enumerable = value as IEnumerable;
			if (enumerable != null) {
				var i = 0;
				foreach (var element in enumerable) {
					var results = element.Validate();
					if (results.Any()) {
						return
							new ValidationResult(
								string.Format("Validation for list {1}[{0}] in {1} failed: {2}", i, validationContext.DisplayName,
									string.Join(" ", results)));
					}
					i++;
				}
			} else {
				var results = value.Validate();
				if (results.Any()) {
					return new ValidationResult(
						string.Format("Validation for object {{{0}}} failed: {1}", validationContext.DisplayName,
							string.Join(" ", results)));
				}
			}

			return ValidationResult.Success;
		}
	}

	public class SIRangeAttribute : RangeAttribute
	{
		public SIRangeAttribute(int minimum, int maximum) : base(minimum, maximum) {}
		public SIRangeAttribute(double minimum, double maximum) : base(minimum, maximum) {}
		public SIRangeAttribute(SI minimum, SI maximum) : base(minimum.Value(), maximum.Value()) {}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			return base.IsValid(((SI)value).Value(), validationContext);
		}
	}

	public class PathAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (!File.Exists((string)value)) {
				return new ValidationResult("File not found: " + (string)value);
			}
			return ValidationResult.Success;
		}
	}
}