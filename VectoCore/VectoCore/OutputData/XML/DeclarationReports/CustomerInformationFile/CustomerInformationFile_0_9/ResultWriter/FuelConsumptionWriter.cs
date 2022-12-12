using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class FuelConsumptionWriterBase : AbstractResultGroupWriter, IFuelConsumptionWriter
	{
		protected FuelConsumptionWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IFuelConsumptionWriter

		public XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fc)
		{
			return new XElement(Cif + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fc.Fuel.FuelType.ToXMLFormat()),
				GetFuelConsumption(fc.TotalFuelConsumptionCorrected, fc.Fuel, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x => new XElement(Cif + XMLNames.Report_Results_FuelConsumption, XMLHelper.ValueAsUnit(x, 3, 1)))
			);
		}

		public XElement GetElement(IWeightedResult entry, IFuelProperties fuel, Kilogram consumption)
		{
			return new XElement(Cif + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumption(consumption, fuel, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x => new XElement(Cif + XMLNames.Report_Results_FuelConsumption, XMLHelper.ValueAsUnit(x, 3, 1)))
			);
		}

		protected abstract IList<ConvertedSI> GetFuelConsumption(Kilogram fc,
			IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume,
			double? passenger);

		#endregion

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion

	}

	public class LorryFuelConsumptionWriter : FuelConsumptionWriterBase
	{
		public LorryFuelConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of FuelConsumptionWriterBase

		protected override IList<ConvertedSI> GetFuelConsumption(Kilogram fc, IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
		{
			var retVal =  new List<ConvertedSI> {
				(fc / distance).ConvertToGrammPerKiloMeter(),
				(fc / distance /payload).ConvertToGrammPerTonKilometer(),
				(fc / distance / volume).ConvertToGrammPerCubicMeterKiloMeter(),

				(fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
				(fc * fuel.LowerHeatingValueVecto / distance / payload).ConvertToMegaJoulePerTonKiloMeter(),
				(fc * fuel.LowerHeatingValueVecto / distance / volume).ConvertToMegaJoulePerCubicMeterKiloMeter(),
			};

			if (fuel.FuelDensity != null) {
				retVal.AddRange(new[] {
					(fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
					(fc / fuel.FuelDensity / distance / payload).ConvertToLiterPerTonKiloMeter(),
					(fc / fuel.FuelDensity / distance /volume).ConvertToLiterPerCubicMeterKiloMeter(),
				});
			}

			return retVal;
		}

		#endregion
	}

	public class BusFuelConsumptionWriter : FuelConsumptionWriterBase
	{
		public BusFuelConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of FuelConsumptionWriterBase

		protected override IList<ConvertedSI> GetFuelConsumption(Kilogram fc, IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
		{
			var retVal = new List<ConvertedSI> {
				(fc / distance).ConvertToGrammPerKiloMeter(),
				(fc / distance / passenger.Value).ConvertToGrammPerPassengerKilometer(),

				(fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
				(fc * fuel.LowerHeatingValueVecto / distance / passenger.Value).ConvertToMegaJoulePerPassengerKilometer(),
			};

			if (fuel.FuelDensity != null) {
				retVal.AddRange(new[] {
					(fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
					(fc / fuel.FuelDensity / distance / passenger.Value).ConvertToLiterPerPassengerKiloMeter(),
				});
			}

			return retVal;
		}

		#endregion
	}
}