using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class CifSummaryWriterBase : AbstractResultWriter, ICifSummaryWriter
	{
		protected CifSummaryWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

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
			return new XElement(Cif + XMLNames.Report_Results_Summary,
				new XAttribute(xsi + XMLNames.XSIType, ResultSummaryXMLType),
				GetSummary(weighted),
				weighted.FuelConsumption.Select(x =>
					FuelConsumptionWriter?.GetElement(weighted, x.Key, x.Value)).ToArray(),
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

	public abstract class LorrySummaryWriterBase : CifSummaryWriterBase
	{
		protected LorrySummaryWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(Cif + "AveragePayload", XMLHelper.ValueAsUnit(weighted.Payload, "t"))
			};
		}
	}

	public class LorryConvSummaryWriter : LorrySummaryWriterBase
	{
		public LorryConvSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultSummaryXMLType => "ResultSummaryConventionalType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionLorry();
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultLorry();
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVNonOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVNonOVCSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultSummaryXMLType => "ResultSummaryNonOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionLorry();
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultLorry();
		protected override IElectricRangeWriter ElectricRangeWriter => null;
		
	}

	public class LorryHEVOVCSummaryWriter : LorrySummaryWriterBase
	{
		public LorryHEVOVCSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionLorry();
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _cifFactory.GetElectricEnergyConsumptionLorry();
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultLorry();
		protected override IElectricRangeWriter ElectricRangeWriter => _cifFactory.GetElectricRangeWriter();

		#endregion
	}

	public class LorryPEVSummaryWriter : LorrySummaryWriterBase
	{
		public LorryPEVSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultSummaryXMLType => "ResultSummaryPEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _cifFactory.GetElectricEnergyConsumptionLorry();
		protected override ICO2Writer CO2Writer => null;
		protected override IElectricRangeWriter ElectricRangeWriter => _cifFactory.GetElectricRangeWriter();

	}

	// ---- bus

	public abstract class BusSummaryWriterBase : CifSummaryWriterBase
	{
		protected BusSummaryWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		protected override XElement[] GetSummary(IWeightedResult weighted)
		{
			return new[] {
				new XElement(Cif + "AveragePassengerCount", weighted.PassengerCount.Value.ToXMLFormat(2))
			};
		}
	}

	public class BusOVCCifSummaryWriter : BusSummaryWriterBase
	{
		public BusOVCCifSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of CifSummaryWriterBase

		public override string ResultSummaryXMLType => "ResultSummaryOVCHEVType";
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionBus();

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _cifFactory.GetElectricEnergyConsumptionBus();
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultBus();
		protected override IElectricRangeWriter ElectricRangeWriter => _cifFactory.GetElectricRangeWriter();

		#endregion
	}
}