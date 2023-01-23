using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusSuperCapData
	{
		public static Ohm InternalResistance = 0.015.SI<Ohm>();
		public static Volt ReferenceMaximumVoltage =  2.7.SI<Volt>();
		public static Farad CapacitanceReference = 3000.SI<Farad>();
		
		public SuperCapData CreateGenericSuperCapData(ISuperCapDeclarationInputData superCapData, double initialSoc)
		{
			if (superCapData == null)
				return null;

			return new SuperCapData {
				InternalResistance = GetInternalResistance(superCapData),
				MaxVoltage = superCapData.MaxVoltage,
				MinVoltage = superCapData.MinVoltage,
				MaxCurrentDischarge = superCapData.MaxCurrentDischarge,
				MaxCurrentCharge = superCapData.MaxCurrentCharge,
				Capacity = superCapData.Capacity,
				InitialSoC = initialSoc
			};
		}

		private Ohm GetInternalResistance(ISuperCapDeclarationInputData superCapData)
		{
			return (InternalResistance.Value() *
					((superCapData.MaxVoltage.Value() - superCapData.MinVoltage.Value()) /
					(0.55 * ReferenceMaximumVoltage.Value())) *
					(CapacitanceReference.Value() / superCapData.Capacity.Value())).SI<Ohm>();
		}
	}
}
