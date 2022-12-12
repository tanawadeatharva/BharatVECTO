using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class CO2WriterBase : AbstractResultWriter, ICO2Writer
	{
		protected CO2WriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IFuelConsumptionWriter

		public XElement[] GetElement(IResultEntry entry)
		{
			return GetFuelConsumption(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
				new XElement(Cif + XMLNames.Report_Results_CO2, XMLHelper.ValueAsUnit(x, 3, 1))).ToArray();
		}

		protected abstract IList<ConvertedSI> GetFuelConsumption(Kilogram co2, Meter distance, Kilogram payload, CubicMeter volume, double? passengers);

		#endregion

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IOVCResultEntry ovcEntry)
		{
			var entry = ovcEntry.Weighted;
			return GetFuelConsumption(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
				new XElement(Cif + XMLNames.Report_Results_CO2, XMLHelper.ValueAsUnit(x, 3, 1))).ToArray();
		}

		#endregion

	}

	public class LorryCO2Writer : CO2WriterBase
	{
		public LorryCO2Writer(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of FuelConsumptionWriterBase

		protected override IList<ConvertedSI> GetFuelConsumption(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(CO2Total / distance).ConvertToGrammPerKiloMeter(),
				(CO2Total / distance / payload).ConvertToGrammPerTonKilometer(),
				(CO2Total / distance / volume).ConvertToGrammPerCubicMeterKiloMeter(),
			};
		}

		#endregion
	}

	public class BusCO2Writer : CO2WriterBase
	{
		public BusCO2Writer(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of FuelConsumptionWriterBase

		protected override IList<ConvertedSI> GetFuelConsumption(Kilogram CO2Total, Meter distance, Kilogram payload, CubicMeter volume, double? passengers)
		{
			return new[] {
				(CO2Total / distance).ConvertToGrammPerKiloMeter(),
				(CO2Total / distance / passengers.Value).ConvertToGrammPerPassengerKilometer(),
			};
		}

		#endregion
	}
}