using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace VECTO3GUI2020.Behaviours
{
	public class PopUpHorizontalAlignmentBehavior : Behavior<Popup>
	{

		private FrameworkElement _placementTarget;
		private double _initialHorizontalOffset;
		private double _popUpWidth;


		private void _placementTarget_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.WidthChanged) {
				SetHorizontalAlignment(e.NewSize.Width, this.AssociatedObject.ActualWidth);
			}
		}

		private void SetHorizontalAlignment(double placeMentTargetActualWidth, double popUpActualWidth)
		{
			if (popUpActualWidth > placeMentTargetActualWidth) {
				this.AssociatedObject.HorizontalOffset = placeMentTargetActualWidth - popUpActualWidth;
			}
		}

		#region Overrides of Behavior


		protected override void OnAttached()
		{
			_placementTarget = this.AssociatedObject.PlacementTarget as FrameworkElement;
			_popUpWidth = this.AssociatedObject.MinWidth;
			_initialHorizontalOffset = this.AssociatedObject.HorizontalOffset;
            _placementTarget.SizeChanged += _placementTarget_SizeChanged;
            this.AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
			SetHorizontalAlignment(_placementTarget.ActualWidth, this.AssociatedObject.ActualWidth);
			base.OnAttached();
		}

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			_popUpWidth = e.NewSize.Width;
            SetHorizontalAlignment(_placementTarget.ActualWidth, e.NewSize.Width);
        }

        protected override void OnDetaching()
		{
			_placementTarget.SizeChanged -= _placementTarget_SizeChanged;
			this.AssociatedObject.HorizontalOffset = _initialHorizontalOffset;
			base.OnDetaching();
		}



		#endregion
	}
}