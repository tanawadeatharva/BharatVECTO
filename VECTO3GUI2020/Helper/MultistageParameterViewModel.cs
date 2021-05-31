using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using Castle.Core.Internal;
using Castle.Core.Resource;
using Castle.DynamicProxy.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.Views.Multistage.CustomControls;

namespace VECTO3GUI2020.Helper
{
	public enum ViewMode
	{
		COMBOBOX,
		TEXTBOX,
		CHECKBOX
	}


	public interface IMultistageParameterViewModel
	{
		bool EditingEnabled { get; set; }
		object CurrentContent { get; set; }
		object DummyContent { get; set; }
		ObservableCollection<Enum> AllowedItems { get; set; }
		object PreviousContent { get; set; }
		Action<MultistageParameterViewModel> EditingChangedCallback { get; set; }
	}

	public class MultistageParameterViewModel : ViewModelBase, IDataErrorInfo, IMultistageParameterViewModel
	{
		private bool _editingEnabled;
		private object _currentContent;
		private object _dummyContent;
		private bool _mandatory;
		private object _storedContent;
		private string _label;
		private ObservableCollection<Enum> _allowedItems;
		private ObservableCollection<Enum> _generatedItems;

		private readonly Action<MultistageParameterViewModel> _propertyChangedCallback;
		private Action<MultistageParameterViewModel> _editingChangedCallback;
		private readonly PropertyInfo _propertyInfo;
		private readonly IViewModelBase _parentViewModel;
		private ViewMode _viewMode;
		private object _previousContent;
		private bool _showCheckBox;

		private Type _type;

		private Type _underlyingTargetType;
		private readonly string _propertyName;

		public MultistageParameterViewModel (
			string propertyName,
			object previousContent,
			IViewModelBase parentViewModel,
			ViewMode viewMode = ViewMode.TEXTBOX,
			object dummyContent = null,
			bool mandatory = false,
			Action<MultistageParameterViewModel> propertyChangedCallback = null,
			params ResourceManager[] resourceManagers)
		{
			_propertyName = propertyName;
			PreviousContent = previousContent;
			_propertyChangedCallback = propertyChangedCallback;
			_parentViewModel = parentViewModel;
			_propertyInfo = parentViewModel.GetType().GetProperty(propertyName);
			
			_viewMode = viewMode;
			_dummyContent = dummyContent;
			_mandatory = mandatory;
			_type = _propertyInfo.PropertyType;
			_label = NameResolver.ResolveName(propertyName, resourceManagers);

			if (dummyContent == null) {
				DummyContent = CreateDummyContent();
			}


			if (Mode == ViewMode.COMBOBOX) {
				if (DummyContent is Enum dummyEnum)
				{
					var enType = dummyEnum.GetType();
					GeneratedListItems = new ObservableCollection<Enum>(Enum.GetValues(enType).Cast<Enum>().ToList());
				}
				else if (CurrentContent is Enum contentEnum)
				{
					var enType = contentEnum.GetType();
					GeneratedListItems = new ObservableCollection<Enum>(Enum.GetValues(enType).Cast<Enum>().ToList());
				}
			}

			CurrentContent = _propertyInfo.GetValue(parentViewModel);
		}

		private object CreateDummyContent()
		{
			var type = _type;
			

			try
			{
				//dynamic dynType = type;
				
				var baseType = type.BaseType;
				//Create SI Dummy
				if (baseType?.BaseType != null && baseType.BaseType == typeof(SI)) {
					var createMethod = baseType.GetMethod("Create");
					var dummyContent = createMethod?.Invoke(null, new object[] { (new double()) });
					return dummyContent;
				}

				var underlyingType = Nullable.GetUnderlyingType(type);
				if (underlyingType == typeof(bool)) {
					_viewMode = ViewMode.CHECKBOX;
					var dummyContent = false;
					return dummyContent;
				}
				
				


				if (type.IsEnum || (underlyingType != null && underlyingType.IsEnum)) {
					_viewMode = ViewMode.COMBOBOX;
				}

				if (Mode == ViewMode.COMBOBOX) {
					Enum dummyEnum;

					try
					{
						if (underlyingType != null)
						{
							dummyEnum = (Enum)Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
						}
						else
						{
							dummyEnum = (Enum)Enum.Parse(type, type.GetEnumNames()[0]);
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.Message);
						return null;
					}
					return dummyEnum;
				}

				Type primitiveType = null;

				if (underlyingType != null && underlyingType.IsValueType) {
					primitiveType = underlyingType;
				} else if (type.IsValueType) {
					primitiveType = type;
				}
				

				if (primitiveType != null) {
					var dummyContent = Convert.ChangeType(0, primitiveType);
					return dummyContent;
				}

				if (type == typeof(string)) {
					return "";
				}
				



			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
				return null;
			}

			return null;
		}



		public bool EditingEnabled
		{
			get => _editingEnabled;
			set
			{
				var old_value = _editingEnabled;
				var new_value = value;
				_editingEnabled = value;
				if (old_value != new_value){
					_editingChangedCallback?.Invoke(this);
					if (new_value == false)
					{
						if (StoredContent != CurrentContent) {
							StoredContent = CurrentContent;
						}
						CurrentContent = null;
					}
					else
					{
						if (StoredContent != null) {
							CurrentContent = StoredContent;
						} else {
							if (Mode == ViewMode.TEXTBOX) {
								CurrentContent = DummyContent;
							}
						}
					}
				}
				OnPropertyChanged(nameof(CurrentContent));
				OnPropertyChanged(nameof(EditingEnabled));
			}
		}

		public object CurrentContent
		{
			get => _currentContent;
			set
			{
				if (value != null) { 
					EditingEnabled = true;
				} 

				var convertedValue = value;

				//Convert value if neccessary
				if (value != null) {
					if (DummyContent != null) {
						convertedValue = Convert.ChangeType(value, DummyContent.GetType());
					}
				}

				if (SetProperty(ref _currentContent, convertedValue)) {
					_propertyInfo.SetValue(_parentViewModel, _currentContent);
					_propertyChangedCallback?.Invoke(this);
				};
			}
		}



		public object DummyContent
		{
			get => _dummyContent;
			set
			{
				//_propertyChangedCallback?.Invoke(this);
				SetProperty(ref _dummyContent, value);
			}
		}

		public object StoredContent
		{
			get => _storedContent;
			set
			{
				SetProperty(ref _storedContent, value);
			}
		}

		public string Label
		{
			get => _label;
			set
			{
				SetProperty(ref _label, value);
			}
		}

		public ObservableCollection<Enum> AllowedItems
		{
			get => _allowedItems;
			set
			{
				SetProperty(ref _allowedItems, value);
				OnPropertyChanged(nameof(CurrentContent));
			}
		}

		public ViewMode Mode => _viewMode;

		public ObservableCollection<Enum> GeneratedListItems
		{
			get => _generatedItems;
			set => SetProperty(ref _generatedItems, value);
		}

		public bool Mandatory
		{
			get
			{
				return _mandatory;
			}
			set
			{
				if (SetProperty(ref _mandatory, value)) {
					OnPropertyChanged(nameof(ShowCheckBox));
					EditingEnabled = true;
				};

			}
		}

		public object PreviousContent
		{
			get => _previousContent;
			set => SetProperty(ref _previousContent, value);
		}

		public bool ShowCheckBox
		{
			get => !_mandatory;
		}

		public bool Optional
		{
			get => !_mandatory;
		}

		#region Implementation of IDataErrorInfo

		public string this[string columnName]
		{
			get
			{
				if (!(_parentViewModel is IDataErrorInfo dataErrorInfo)) {
					return null;
				}

				return dataErrorInfo[_propertyName];
			}
		}

		public string Error => throw new NotImplementedException();

		public Action<MultistageParameterViewModel> EditingChangedCallback
		{
			get => _editingChangedCallback;
			set => _editingChangedCallback = value;
		}

		/// <summary>
		/// Sets Editing Enabled flag based on currentvalue
		/// </summary>
		/// <returns></returns>
		public bool UpdateEditingEnabled()
		{
			if (CurrentContent == null) {
				EditingEnabled = false;
			} else {
				EditingEnabled = true;
			}

			return EditingEnabled;
		}
		

		#endregion
	}
}