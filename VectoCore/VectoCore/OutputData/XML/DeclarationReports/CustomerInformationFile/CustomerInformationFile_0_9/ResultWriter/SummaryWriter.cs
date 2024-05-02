using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
    public abstract class SummaryWriterBase : AbstractResultWriter, IReportResultsSummaryWriter
	{
		protected SummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of ICifSummaryWriter

		public XElement GetElement(IList<IResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries);
			return DoGetElement(weighted);
		}

		public XElement GetElement(IList<IOVCResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries);
			return DoGetElement(weighted);
		}

		protected virtual XElement DoGetElement(IWeightedResult weighted)
		{
			if (weighted == null) {
				return null;
			}

			var fcWriter = FuelConsumptionWriter;
			return new XElement(TNS + XMLNames.Report_Results_Summary,
				new XAttribute(xsi + XMLNames.XSIType, ResultSummaryXMLType),
				GetSummary(weighted),
				fcWriter != null
					? weighted.FuelConsumption.Select(x =>
						fcWriter.GetElements(weighted))
					: null,
				ElectricEnergyConsumptionWriter?.GetElement(weighted),
				CO2Writer?.GetElements(weighted),
				ElectricRangeWriter?.GetElements(weighted)
			);
		}

		public abstract string ResultSummaryXMLType { get; }


		protected abstract XElement[] GetSummary(IWeightedResult weighted);

		protected abstract IFuelConsumptionWriter FuelConsumptionWriter { get; }

		protected abstract IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter { get; }

		protected abstract ICO2Writer CO2Writer { get; }

		protected abstract IElectricRangeWriter ElectricRangeWriter { get; }
		
		#endregion
	}

	

	public abstract class LorrySummaryWriterBase : SummaryWriterBase
	{
		protected LorrySummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(TNS + "AveragePayload", XMLHelper.ValueAsUnit(weighted.Payload, "t", 3))
			};
		}
	}

	public class LorryConvSummaryWriter : LorrySummaryWriterBase
	{
		public LorryConvSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryConventionalType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVNonOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryNonOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionLorry(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionLorry(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultLorry(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

	public class LorryPEVSummaryWriter : LorrySummaryWriterBase
	{
		public LorryPEVSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override string ResultSummaryXMLType => "ResultSummaryPEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionLorry(_factory, TNS);
		protected override ICO2Writer CO2Writer => null;
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

	}

	// ---- bus

	public abstract class BusSummaryWriterBase : SummaryWriterBase
	{
		protected BusSummaryWriterBase(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(TNS + "AveragePassengerCount", weighted.PassengerCount.Value.ToXMLFormat(2))
			};
		}
	}

	public class BusConvSummaryWriter : BusSummaryWriterBase
	{
		public BusConvSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryConventionalType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusHEVNonOVCSummaryWriter : BusSummaryWriterBase
	{
		public BusHEVNonOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryNonOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusHEVOVCSummaryWriter : BusSummaryWriterBase
	{
		public BusHEVOVCSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

	public class BusPEVSummaryWriter : BusSummaryWriterBase
	{
		public BusPEVSummaryWriter(ICIFResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryPEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);
		protected override ICO2Writer CO2Writer => _factory.GetCO2SummaryResultPEVBus(_factory, TNS);
		protected override IElectricRangeWriter ElectricRangeWriter => _factory.GetElectricRangeWriter(_factory, TNS);

		#endregion
	}

}