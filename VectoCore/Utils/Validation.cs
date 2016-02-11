/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Helper Class for doing the Validation
	/// </summary>
	public static class ValidationHelper
	{
		/// <summary>
		/// Validates the specified entity and all its properties recursively. (Extension Method)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns>Null, if the validation was successfull. Otherwise a list of ValidationResults with the ErrorMessages.</returns>
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

		/// <summary>
		/// Determines whether this instance is valid. (Extension Method)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static bool IsValid<T>(this T entity)
		{
			return Validator.TryValidateObject(entity, new ValidationContext(entity), null, true);
		}
	}

	/// <summary>
	/// Determines that the attributed object should be validated recursively.
	/// </summary>
	public class ValidateObjectAttribute : ValidationAttribute
	{
		/// <summary>
		/// Validates an object recursively (all elements if its a list, or all fields and properties if its an object).
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull. Otherwise the joined ErrorMessages are returned.
		/// </returns>
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
						return new ValidationResult(
							string.Format("Validation for list {1}[{0}] in {1} failed: {2}", i, validationContext.DisplayName,
								string.Concat(results)));
					}
					i++;
				}
			} else {
				var results = value.Validate();
				if (results.Any()) {
					return new ValidationResult(
						string.Format("Validation for object {{{0}}} failed: {1}", validationContext.DisplayName, string.Concat(results)));
				}
			}

			return ValidationResult.Success;
		}
	}

	/// <summary>
	/// Attribute which validates the Min-Max Range of an SI Object.
	/// </summary>
	public class SIRangeAttribute : RangeAttribute
	{
		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(int minimum, int maximum) : base(minimum, maximum) {}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(double minimum, double maximum) : base(minimum, maximum) {}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(SI minimum, SI maximum) : base(minimum.Value(), maximum.Value()) {}

		/// <summary>
		/// Validates that an SI Object is inside the min-max range.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull, otherwise an Instance of ValidationResult with the ErrorMessage.
		/// </returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			return base.IsValid(((SI)value).Value(), validationContext);
		}
	}

	/// <summary>
	/// Attribute which validates a Path.
	/// </summary>
	public class PathAttribute : ValidationAttribute
	{
		/// <summary>
		/// Validates that a path actually exists.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull, otherwise an Instance of ValidationResult with the ErrorMessage.
		/// </returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (!File.Exists((string)value)) {
				return new ValidationResult("File not found: " + (string)value);
			}
			return ValidationResult.Success;
		}
	}
}