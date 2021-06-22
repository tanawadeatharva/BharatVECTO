using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace VECTO3GUI2020.Behaviours
{
	public class MinSizeBehavior : Behavior<Window>
	{
		#region Overrides of Behavior

		private double _initialMinWidth;
		private double _initialMinHeight;

		protected override void OnAttached()
		{
			this.AssociatedObject.Activated += OnWindowActivated;
			_initialMinHeight = this.AssociatedObject.MinHeight;
			_initialMinWidth = this.AssociatedObject.MinWidth;
			base.OnAttached();
			
		}

		private void OnWindowActivated(object sender, EventArgs e)
		{
			for(var i = 0; i < VisualTreeHelper.GetChildrenCount((Window)sender); i++)
			{
				var child = VisualTreeHelper.GetChild((Window)sender, 0) as FrameworkElement;
				if (child.MinHeight != 0 && this.AssociatedObject.MinHeight == 0) {
					this.AssociatedObject.MinHeight = child.MinHeight;
				}
				if (child.MinWidth != 0 && this.AssociatedObject.MinWidth == 0)
				{
					this.AssociatedObject.MinWidth = child.MinWidth;
				}
			}



		}

		private void OnWindowInitialized(object sender, EventArgs e)
		{
	

			

		}

		protected override void OnDetaching()
		{


			this.AssociatedObject.Initialized -= OnWindowActivated;

			base.OnDetaching();

		}

		#endregion
	}
}