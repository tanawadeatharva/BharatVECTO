using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Build.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Views.CustomControls;

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{

	public enum MultistageParameterViewMode
	{
		TEXTBOX,
		CHECKBOX,
		COMBOBOX
	}


	/// <summary>
	/// Interaction logic for MultiStageParameter.xaml
	/// </summary>
	public partial class MultiStageParameter : UserControl, INotifyPropertyChanged
    {

		#region Dependency Properties


		public static readonly DependencyProperty PreviousContentProperty = DependencyProperty.Register(
			"PreviousContent", typeof(object), typeof(MultiStageParameter), new PropertyMetadata(default(object)));

		public object PreviousContent
		{
			get { return (object)GetValue(PreviousContentProperty); }
			set { SetValue(PreviousContentProperty, value); }
		}


		public static readonly DependencyProperty OptionalProperty = DependencyProperty.Register(
			"Optional", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(true, new PropertyChangedCallback(OptionalChanged)));

		private static void OptionalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiStageParameter multiStageParameter = (MultiStageParameter)d;
			if ((bool)e.NewValue == false) {
				multiStageParameter.EditingEnabled = true;
				multiStageParameter.ShowCheckBox = false;
			} else {
				multiStageParameter.ShowCheckBox = true;
			}
		}

		public bool Optional
		{
			get { return (bool)GetValue(OptionalProperty); }
			set
			{
				SetValue(OptionalProperty, value);
			}
		}

		public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(MultistageParameterViewMode), typeof(MultiStageParameter), new PropertyMetadata(MultistageParameterViewMode.TEXTBOX));

        public MultistageParameterViewMode Mode
		{
			get { return (MultistageParameterViewMode)GetValue(ModeProperty); }
			set { SetValue(ModeProperty, value); }
		}

		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
			"Label", typeof(string), typeof(MultiStageParameter), new PropertyMetadata(default(string)));

		public string Label
		{
			get { return (string)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}


		public static readonly DependencyProperty GeneratedLabelTextProperty = DependencyProperty.Register(
			"GeneratedLabelText", typeof(string), typeof(MultiStageParameter), new PropertyMetadata(null));

		public string GeneratedLabelText
		{
			get { return (string)GetValue(GeneratedLabelTextProperty); }
			set { SetValue(GeneratedLabelTextProperty, value); }
		}

		public static readonly DependencyProperty NameLookUpResourceManagerProperty = DependencyProperty.Register(
			"NameLookUpResourceManager", typeof(ResourceManager), typeof(MultiStageParameter), new PropertyMetadata(default(ResourceManager), NameLookUpResourceManagerChanged));

		private static void NameLookUpResourceManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var multistageParameter = (MultiStageParameter)d;
			multistageParameter.SetLabelText();
		}

		public ResourceManager NameLookUpResourceManager
		{
			get { return (ResourceManager)GetValue(NameLookUpResourceManagerProperty); }
			set { SetValue(NameLookUpResourceManagerProperty, value); }
		}


		public static readonly DependencyProperty EditingEnabledProperty = DependencyProperty.Register(
			"EditingEnabled", typeof(bool), typeof(MultiStageParameter), new FrameworkPropertyMetadata(
				false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EditingEnabledChanged)));

		public bool EditingEnabled
		{
			get
			{
				return (bool)GetValue(EditingEnabledProperty);
			}
			set
			{
				SetValue(EditingEnabledProperty, value);
			}
		}

		public static readonly DependencyProperty DummyContentProperty = DependencyProperty.Register(
			"DummyContent", typeof(object), typeof(MultiStageParameter),
			new FrameworkPropertyMetadata(null));

		public object DummyContent
		{
			get { return (object)GetValue(DummyContentProperty); }
			set { SetValue(DummyContentProperty, value); }
		}


		public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register(
			"ShowCheckBox", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(true));

		public bool ShowCheckBox
		{
			get { return (bool)GetValue(ShowCheckBoxProperty); }
			set { SetValue(ShowCheckBoxProperty, value); }
		}


		// Using a DependencyProperty as the backing store for MyObject.  This enables animation, styling, binding, etc...
		public new static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
				typeof(object),
				typeof(MultiStageParameter),
				new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ContentChanged)); //TODO Default value null breaks it for SIs default value "" for strings

		public new object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set
			{
				SetCurrentValue(ContentProperty, value);
			}
		}

		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			"ListItems", typeof(ObservableCollection<Enum>), typeof(MultiStageParameter),
			new FrameworkPropertyMetadata(default(ObservableCollection<Enum>), ListItemsChanged));

		private static void ListItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			//throw new NotImplementedException();
		}

		public ObservableCollection<Enum> ListItems
		{
			get { return (ObservableCollection<Enum>)GetValue(ListItemsProperty); }
			set { SetValue(ListItemsProperty, value); }
		}
		

		public static readonly DependencyProperty GeneratedListItemsProperty = DependencyProperty.Register(
			"GeneratedListItems", typeof(List<object>), typeof(MultiStageParameter), new PropertyMetadata(defaultValue:null));

		public List<object> GeneratedListItems
		{
			get { return (List<object>)GetValue(GeneratedListItemsProperty); }
			set { SetValue(GeneratedListItemsProperty, value); }
		}



#endregion

		private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var multiStageParameter = (CustomControls.MultiStageParameter) d;

			if (multiStageParameter.DummyContent == null) {
				multiStageParameter.DummyContent = multiStageParameter.CreateDummyContent(e, multiStageParameter);
			}

			if (multiStageParameter.Content != null) {
				multiStageParameter.EditingEnabled = true;
			}

			if (multiStageParameter.Mode == MultistageParameterViewMode.COMBOBOX) {

				multiStageParameter.GenerateListItemsAndSetComboboxValue();
			}


			if (multiStageParameter.GeneratedLabelText == null) {
				multiStageParameter.SetLabelText();
			}
		}

		public void SetLabelText()
		{
			if (Label == null) {
				this.GeneratedLabelText = this.GetLabelByPropertyName(
					MultiStageParameter.ContentProperty,
					this.NameLookUpResourceManager,
					Strings.ResourceManager);
			}
		}

		private void GenerateListItemsAndSetComboboxValue()
		{
			if (Mode != MultistageParameterViewMode.COMBOBOX) 
				return;

			if (GeneratedListItems == null) {
				if (DummyContent is Enum dummyEnum)
				{
					var enType = dummyEnum.GetType();
					GeneratedListItems = Enum.GetValues(enType).Cast<object>().ToList();


				}

				if (Content is Enum contentEnum)
				{
					var enType = contentEnum.GetType();
					GeneratedListItems = Enum.GetValues(enType).Cast<object>().ToList();

				}

				//ListItems = GeneratedListItems;
			}
		}

		private object CreateDummyContent(DependencyPropertyChangedEventArgs e, UserControl userControl)
		{
			var type = userControl.GetPropertyType(e.Property);
			if (type == null) {
				return null;
			}

			if (type == typeof(ConvertedSI)) {
				//var dummyContent = new ConvertedSI(0, (userControl.Content as ConvertedSI).Units);
				return null; //dummyContent;
			}
			try {
				dynamic dynType = type;
				var baseType = dynType.BaseType;
				//Create SI Dummy

				


				if (baseType?.BaseType != null && baseType.BaseType == typeof(SI)) {
					var createMethod = baseType.GetMethod("Create");
					var dummyContent = createMethod?.Invoke(null, new object[] { (new double()) });
					return dummyContent;
				}else if(Mode == MultistageParameterViewMode.COMBOBOX) {
					var bindingProperty = userControl.GetBindingExpression(e.Property);
					var dataItemType = bindingProperty?.DataItem?.GetType();
					var sourcePropertyType =
						dataItemType?.GetProperty(bindingProperty?.ResolvedSourcePropertyName)?.PropertyType;

					var underlyingType = Nullable.GetUnderlyingType(dynType);
					Enum dummyEnum;
					if (underlyingType != null) {
						dummyEnum = Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
                    } else {
						dummyEnum = Enum.Parse(dynType, dynType.GetEnumNames()[0]);
					}

					

					return dummyEnum;
				}
			
			} catch (Exception ex) {
				Debug.WriteLine(ex.Message);
				return null;
			}

			return null;
		}

		private static void EditingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			MultiStageParameter multiStageParameter = (MultiStageParameter)d;
			if((bool)e.NewValue == false) {
				if (!Validation.GetHasError(multiStageParameter.TextBoxContent)) {
					multiStageParameter.DummyContent = multiStageParameter.Content;
				}
				multiStageParameter.Content = null;
            } else {
				if (multiStageParameter.Content != null) {
					multiStageParameter.DummyContent = multiStageParameter.Content;
				}else if (multiStageParameter.DummyContent != null) {
					if (multiStageParameter.Mode == MultistageParameterViewMode.COMBOBOX) {
						if (multiStageParameter.ListItems != null && !multiStageParameter.ListItems.Contains(multiStageParameter.DummyContent)) {
							multiStageParameter.Content = multiStageParameter.ListItems[0];
						} else {
							multiStageParameter.Content = multiStageParameter.DummyContent;
						}
					} else {
						multiStageParameter.Content = multiStageParameter.DummyContent;
					}
					
				}
			}
		}







		public MultiStageParameter()
        {
			InitializeComponent();
		}

		private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			EditingEnabled = true;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
