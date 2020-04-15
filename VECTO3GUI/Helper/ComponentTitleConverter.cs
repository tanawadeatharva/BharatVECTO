using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VECTO3GUI.Util;

namespace VECTO3GUI.Helper
{
	public class ComponentTitleConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Component) {
				var component = (Component)value;
				switch (component) {
					case Component.Vehicle:
					case Component.PrimaryBusVehicle:
					case Component.CompleteBusVehicle:
						return nameof(Component.Vehicle);
					case Component.Engine:
						return nameof(Component.Engine);
					case Component.Gearbox:
						return nameof(Component.Gearbox);
					case Component.TorqueConverter:
						return "Torque Converter";
					case Component.Retarder:
						return "Retarder";
					case Component.Angledrive:
						return "Angle Drive";
					case Component.Axlegear:
						return "Axle Gear";
					case Component.PTO:
						return "Power Take Off";
					case Component.Airdrag:
						return "Air Drag";
					case Component.Axles:
						return "Axle";
					case Component.Auxiliaries:
					case Component.BusAuxiliaries:
						return "Auxiliary";
					case Component.Cycle:
						return nameof(Component.Cycle);
				}
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
