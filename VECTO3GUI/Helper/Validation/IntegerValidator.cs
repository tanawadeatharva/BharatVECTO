using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.Views.CustomControls;

namespace VECTO3GUI.Helper.Validation
{
	public class IntegerValidator : ValidationRule
	{
		private IntegerValidatorConfig _integerValidator;

		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public bool ValidateInput { get; set; }


		public IntegerValidator()
		{
			
		}

		public IntegerValidatorConfig ValidatorConfig
		{
			get { return _integerValidator; }
			set
			{
				_integerValidator = value;
				value?.SetValidator(this);
			}
		}

		//public override ValidationResult Validate(object value, CultureInfo cultureInfo, BindingExpressionBase owner)
		//{
		//	var validateResult = base.Validate(value, cultureInfo, owner);

		//	var dataItem = ((IntegerVectoParameterControl)((BindingExpression)owner).DataItem);


		//	if (dataItem.DataContext is CompleteVehicleBusViewModel) {

		//	}




		//	//var exp = ((IntegerVectoParameterControl)((BindingExpression)owner).DataItem).Caption;


		//	//var t = ((IntegerVectoParameterControl)((BindingExpression)owner).ResolvedSource) .DataContext as CompleteVehicleBusViewModel;
		//	//t.InputValidationErrors = !validateResult.IsValid;
			
		//	return validateResult;

		//}

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{


			var strValue = value as string;
			int number;

			if(!int.TryParse(strValue, out number))
				return new ValidationResult(false, "Not a valid integer value!");

			if (!ValidateInput)
				return ValidationResult.ValidResult;


			if (number < MinValue)
				return new ValidationResult(false, $"Only integer values greater than or equals to {MinValue} are allowed!");

			if(number > MaxValue)
				return new ValidationResult(false, $"Only integer values less than or equals to {MaxValue} are allowed!");

			return ValidationResult.ValidResult;
		}

		private void SetPropertyError(bool validationResult, string propertyName)
		{

			
		}




		//protected void SetChangedProperty(bool changed, [CallerMemberName] string propertyName = "")
		//{
		//	if (!changed)
		//	{
		//		if (_changedInput.Contains(propertyName))
		//			_changedInput.Remove(propertyName);
		//	}
		//	else
		//	{
		//		if (!_changedInput.Contains(propertyName))
		//			_changedInput.Add(propertyName);
		//	}

		//	UnsavedChanges = _changedInput.Count > 0;
		//}
	}
}
