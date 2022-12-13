using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.
	ResultWriter
{

	public abstract class ElectricEnergyConsumptionWriterBase : AbstractResultWriter, IElectricEnergyConsumptionWriter
	{
		public ElectricEnergyConsumptionWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "ElectricEnergy",
				GetEnergyConsumption(entry.ElectricEnergyConsumption, entry.Distance, entry.Payload, entry.CargoVolume,
					entry.PassengerCount).Select(x =>
					new XElement(Cif + XMLNames.Report_Result_EnergyConsumption, XMLHelper.ValueAsUnit(x, 3, 1)))
			);
		}

		public virtual XElement GetElement(IWeightedResult weighted)
		{
			return new XElement(Cif + "ElectricEnergy",
				GetEnergyConsumption(weighted.ElectricEnergyConsumption, weighted.Distance, weighted.Payload, weighted.CargoVolume,
					weighted.PassengerCount).Select(x =>
					new XElement(Cif + XMLNames.Report_Result_EnergyConsumption, XMLHelper.ValueAsUnit(x, 3, 1)))
			);

		}

		protected abstract IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
			Kilogram payload, CubicMeter volume, double? passengers);

		#endregion
	}

	public class LorryElectricEnergyConsumptionWriter : ElectricEnergyConsumptionWriterBase
	{
		public LorryElectricEnergyConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of ElectricEnergyConsumptionWriterBase

		protected override IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
			Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(elEnergy / distance).ConvertToKiloWattHourPerKiloMeter(),
				(elEnergy / distance / payload).ConvertToKiloWattHourPerTonKiloMeter(),
				(elEnergy / distance / volume).ConvertToKiloWattHourPerCubicMeterKiloMeter(),

				(elEnergy / distance).ConvertToMegaJoulePerKiloMeter(),
				(elEnergy / distance / payload).ConvertToMegaJoulePerTonKiloMeter(),
				(elEnergy / distance / volume).ConvertToMegaJoulePerCubicMeterKiloMeter(),
			};
		}

		#endregion
	}

	public class BusElectricEnergyConsumptionWriter : ElectricEnergyConsumptionWriterBase
	{
		public BusElectricEnergyConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of ElectricEnergyConsumptionWriterBase

		protected override IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
			Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(elEnergy / distance).ConvertToKiloWattHourPerKiloMeter(),
				(elEnergy / distance / passengers.Value).ConvertToKiloWattHourPerPassengerKiloMeter(),

				(elEnergy / distance).ConvertToMegaJoulePerKiloMeter(),
				(elEnergy / distance / passengers.Value).ConvertToMegaJoulePerPassengerKiloMeter(),
			};
		}

		#endregion
	}
}