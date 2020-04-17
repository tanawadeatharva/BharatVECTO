namespace VECTO3GUI.Util {
	public enum Component
	{
		Vehicle = 1,
		PrimaryBusVehicle,
		Engine,
		Gearbox,
		TorqueConverter,
		Retarder,
		Angledrive,
		Axlegear,
		PTO,
		Airdrag,
		Axles,
		Auxiliaries,
		BusAuxiliaries,
		Cycle,
		CompleteBusVehicle
	}

	public static class ComponentHelper
	{
		public static string GetLabel(this Component component)
		{
			switch (component)
			{
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

			return string.Empty;
		}
	}
}