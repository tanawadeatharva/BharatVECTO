using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VECTO3GUI2020.Views.CustomControls
{
    public static class CustomControlExtensionMethods
    {
		/// <summary>
		/// Looks up the Label by the name the Property that is used for the binding
		/// </summary>
		/// <param name="dependencyProperty"></param>
		/// <param name="resourceManager"></param>
		/// <returns></returns>
		public static string GetLabelByPropertyName(this UserControl userControl, DependencyProperty dependencyProperty, ResourceManager resourceManager)
		{
			var name = "unresolved";
			var binding = userControl.GetBindingExpression(dependencyProperty);
			var propertyName = binding?.ResolvedSourcePropertyName;
			
			if (propertyName == null || binding == null)
			{
				return name;
			}


			var extendedPropertyName = binding?.ResolvedSource.GetType().Name + "_" + propertyName;
			name = resourceManager?.GetString(extendedPropertyName) ?? resourceManager?.GetString(propertyName) ?? (propertyName + "_"); //_Postfix to label Property Names that are not in strings.resx

			return name;
		}

		public static Type GetPropertyType(this UserControl userControl, DependencyProperty dependencyProperty)
		{
			var Binding = userControl.GetBindingExpression(dependencyProperty);
			var PropertyName = Binding?.ResolvedSourcePropertyName;

			if (PropertyName == null || Binding == null) {
				return typeof(object);
			}
			var PropertyType = Binding?.ResolvedSource?.GetType().GetProperty(PropertyName).PropertyType;
			return PropertyType;
		}
	}
}
