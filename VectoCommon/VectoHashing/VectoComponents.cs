using System;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoHashing
{
	public enum VectoComponents
	{
		Engine,
		Gearbox,
		Axlegear,
		Retarder,
		TorqueConverter,
		Angledrive,
		Airdrag,
		Tyre,
		Vehicle,
	}

	public static class VectoComponentsExtensionMethods
	{
		public static string XMLElementName(this VectoComponents component)
		{
			switch (component) {
				case VectoComponents.Engine:
					return XMLNames.Component_Engine;
				case VectoComponents.Gearbox:
					return XMLNames.Component_Gearbox;
				case VectoComponents.Axlegear:
					return XMLNames.Component_Axlegear;
				case VectoComponents.Retarder:
					return XMLNames.Component_Retarder;
				case VectoComponents.TorqueConverter:
					return XMLNames.Component_TorqueConverter;
				case VectoComponents.Angledrive:
					return XMLNames.Component_Angledrive;
				case VectoComponents.Airdrag:
					return XMLNames.Component_AirDrag;
				case VectoComponents.Tyre:
					return XMLNames.AxleWheels_Axles_Axle_Tyre;
				case VectoComponents.Vehicle:
					return XMLNames.Component_Vehicle;
				default:
					throw new ArgumentOutOfRangeException("VectoComponents", component, null);
			}
		}
	}
}