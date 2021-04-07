using System;
using System.Collections.Generic;
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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Properties;
using VECTO3GUI2020.Views.CustomControls;

namespace VECTO3GUI2020.Views.Multistage.CustomControls
{
    /// <summary>
    /// Interaction logic for MultiStageParameter.xaml
    /// </summary>
    public partial class MultiStageParameter : UserControl
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
			}
		}

		public bool Optional
		{
			get { return (bool)GetValue(OptionalProperty); }
			set { SetValue(OptionalProperty, value); }
		}

		public static readonly DependencyProperty ComboBoxModeProperty = DependencyProperty.Register(
			"ComboBoxMode", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(false));

		public bool ComboBoxMode
		{
			get { return (bool)GetValue(ComboBoxModeProperty); }
			set { SetValue(ComboBoxModeProperty, value); }
		}




		public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
			"LabelText", typeof(string), typeof(MultiStageParameter), new PropertyMetadata(null));

		public string LabelText
		{
			get { return (string)GetValue(LabelTextProperty); }
			set { SetValue(LabelTextProperty, value); }
		}

		public static readonly DependencyProperty EditingEnabledProperty = DependencyProperty.Register(
			"EditingEnabled", typeof(bool), typeof(MultiStageParameter), new FrameworkPropertyMetadata(
				false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(EditingEnabledChanged)));



		public bool EditingEnabled
		{
			get { return (bool)GetValue(EditingEnabledProperty);}
			set
			{
				SetValue(EditingEnabledProperty, value);
			}
		}

		public static readonly DependencyProperty DummyContentProperty = DependencyProperty.Register(
			"DummyContent", typeof(object), typeof(MultiStageParameter), new PropertyMetadata(default(object)));

		public object DummyContent
		{
			get { return (object)GetValue(DummyContentProperty); }
			set { SetValue(DummyContentProperty, value); }
		}


		public static readonly DependencyProperty HideCheckBoxProperty = DependencyProperty.Register(
			"HideCheckBox", typeof(bool), typeof(MultiStageParameter), new PropertyMetadata(default(bool)));

		public bool HideCheckBox
		{
			get { return (bool)GetValue(HideCheckBoxProperty); }
			set { SetValue(HideCheckBoxProperty, value); }
		}


		// Using a DependencyProperty as the backing store for MyObject.  This enables animation, styling, binding, etc...
		public new static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content",
				typeof(object),
				typeof(MultiStageParameter),
				new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ContentChanged))); //TODO Default value null breaks it for SIs default value "" for strings

		public new object Content
		{
			get { return (object)GetValue(ContentProperty); }
			set
			{
				SetCurrentValue(ContentProperty, value);
			}
		}

		public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register(
			"ListItems", typeof(List<object>), typeof(MultiStageParameter), new PropertyMetadata(default(List<object>)));

		public List<object> ListItems
		{
			get { return (List<object>)GetValue(ListItemsProperty); }
			set { SetValue(ListItemsProperty, value); }
		}

#endregion

		private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var multiStageParameter = (CustomControls.MultiStageParameter) d;


			multiStageParameter.DummyContent = multiStageParameter.CreateDummyContent(e, multiStageParameter);


			multiStageParameter.SetListItems();


			if (multiStageParameter.LabelText != null) {
				return;
			}
			multiStageParameter.LabelText = multiStageParameter.GetLabelByPropertyName(
				MultiStageParameter.ContentProperty,
				Strings.ResourceManager);
		}

		private void SetListItems()
		{
			if (!ComboBoxMode) 
				return;

			if(DummyContent is Enum en) {
				var enType = en.GetType();

				ListItems = Enum.GetValues(enType).Cast<object>().ToList();
			}
		}

		private object CreateDummyContent(DependencyPropertyChangedEventArgs e, UserControl userControl)
		{
			dynamic type = userControl.GetPropertyType(e.Property);

			var baseType = type.BaseType;
			try {
				//Create SI Dummy

				if (baseType.BaseType == typeof(SI)) {
					var createMethod = baseType.GetMethod("Create");
					var dummyContent = createMethod?.Invoke(null, new object[] { (new double()) });
					return dummyContent;
				} else{
					var bindingProperty = userControl.GetBindingExpression(e.Property);
					var dataItemType = bindingProperty?.DataItem.GetType();
					var sourcePropertyType =
						dataItemType?.GetProperty(bindingProperty?.ResolvedSourcePropertyName)?.PropertyType;

					var underlyingType = Nullable.GetUnderlyingType(type);
					Enum dummyEnum;
					if (underlyingType != null) {
						dummyEnum = Enum.Parse(underlyingType, underlyingType.GetEnumNames()[0]);
                    } else {
						dummyEnum = Enum.Parse(type, type.GetEnumNames()[0]);
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
			if((bool)e.NewValue == false)
			{
				multiStageParameter.Content = null;
            } else {
				if (multiStageParameter.DummyContent != null) {
					multiStageParameter.Content = multiStageParameter.DummyContent;
				}
			}
		}







		public MultiStageParameter()
        {
			InitializeComponent();
			
			//LabelText = this.GetLabelByPropertyName(ContentProperty, Strings.ResourceManager);
		}

		private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			EditingEnabled = true;
		}
	}
}
