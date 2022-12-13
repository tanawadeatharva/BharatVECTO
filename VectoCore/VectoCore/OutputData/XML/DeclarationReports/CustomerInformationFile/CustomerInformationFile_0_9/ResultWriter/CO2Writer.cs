using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class CO2WriterBase : AbstractResultWriter, ICO2Writer
	{
		protected CO2WriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public virtual XElement[] GetElements(IResultEntry entry)
		{
			return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
				new XElement(Cif + XMLNames.Report_Results_CO2, XMLHelper.ValueAsUnit(x, 3, 1))).ToArray();
		}

		public virtual XElement[] GetElements(IWeightedResult entry)
		{
			return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
				new XElement(Cif + XMLNames.Report_Results_CO2, XMLHelper.ValueAsUnit(x, 3, 1))).ToArray();
		}

		protected abstract IList<ConvertedSI> GetCO2ResultEntries(Kilogram co2, Meter distance, Kilogram payload, CubicMeter volume, double? passengers);

	}

	public class LorryCO2Writer : CO2WriterBase
	{
		public LorryCO2Writer(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		protected override IList<ConvertedSI> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(CO2Total / distance).ConvertToGrammPerKiloMeter(),
				(CO2Total / distance / payload).ConvertToGrammPerTonKilometer(),
				(CO2Total / distance / volume).ConvertToGrammPerCubicMeterKiloMeter(),
			};
		}

	}

	public class BusCO2Writer : CO2WriterBase
	{
		public BusCO2Writer(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		protected override IList<ConvertedSI> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(CO2Total / distance).ConvertToGrammPerKiloMeter(),
				(CO2Total / distance / passengers.Value).ConvertToGrammPerPassengerKilometer(),
			};
		}

	}

	public class BusPEVCO2Writer : BusCO2Writer
	{
		public BusPEVCO2Writer(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of ICO2Writer

		public override XElement[] GetElements(IResultEntry entry)
		{
			if (entry.AuxHeaterFuel == null) {
				return null;
			}

			var tmp = _cifFactory.GetFuelConsumptionBus() as BusFuelConsumptionWriter;
			return new[] {
				new XElement(Cif + "FC_ZEV_AuxHeater",
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
					tmp?.FuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
						entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
						new XElement(Cif + XMLNames.Report_Results_FuelConsumption, x.ValueAsUnit(3, 1)))
				),
				new XElement(Cif + "CO2_ZEV_AuxHeater",
					GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume,
						entry.PassengerCount).Select(x =>
						new XElement(Cif + XMLNames.Report_Results_CO2, x.ValueAsUnit(3, 1)))
				)
			};
		}

		public override XElement[] GetElements(IWeightedResult entry)
		{
			if (entry.AuxHeaterFuel == null) {
				return null;
			}

			var tmp = _cifFactory.GetFuelConsumptionBus() as BusFuelConsumptionWriter;
			return new[] {
				new XElement(Cif + "FC_ZEV_AuxHeater",
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
					tmp?.FuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
						entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
						new XElement(Cif + XMLNames.Report_Results_FuelConsumption, x.ValueAsUnit(3, 1)))
				),
				new XElement(Cif + "CO2_ZEV_AuxHeater",
					GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume,
						entry.PassengerCount).Select(x =>
						new XElement(Cif + XMLNames.Report_Results_CO2, x.ValueAsUnit(3, 1)))
				)
			};
		}

		#endregion

	}
}