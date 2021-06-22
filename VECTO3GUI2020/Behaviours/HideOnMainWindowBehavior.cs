using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace VECTO3GUI2020.Behaviours
{
	public class HideOnMainWindowBehavior : Behavior<FrameworkElement>
	{
		#region Overrides of Behavior

		private Visibility savedState;
		protected override void OnAttached()
		{

			base.OnAttached();
			var window = Window.GetWindow(this.AssociatedObject);
			if (window == Application.Current.MainWindow) {
				savedState = AssociatedObject.Visibility;
				this.AssociatedObject.Visibility = Visibility.Hidden;
			}
		}

		protected override void OnDetaching()
		{
			if (savedState != null) {
				this.AssociatedObject.Visibility = savedState;
			}
			base.OnDetaching();
		}

		#endregion
	}
}