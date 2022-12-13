using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class NonOVCTotalWriterBase : AbstractResultGroupWriter
	{
		protected NonOVCTotalWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "Total",
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed,
					XMLHelper.ValueAsUnit(entry.AverageSpeed, "km/h", 1)),
				entry.FuelData.Select(f =>
					FuelConsumptionWriter?.GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				ElectricEnergyConsumptionWriter?.GetElement(entry),
				CO2Writer?.GetElements(entry),
				ElectricRangeWriter?.GetElements(entry)
			);
		}

		#endregion

		protected abstract IFuelConsumptionWriter FuelConsumptionWriter { get; }

		protected abstract IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter { get; }

		protected abstract ICO2Writer CO2Writer { get; }

		protected abstract IElectricRangeWriter ElectricRangeWriter { get; }

	}

	public class LorryConvTotalWriter : NonOVCTotalWriterBase
	{
		public LorryConvTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionLorry();

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;

		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultLorry();

		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class LorryHEVNonOVCTotalWriter : NonOVCTotalWriterBase
	{
		public LorryHEVNonOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionLorry();

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;

		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultLorry();

		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class LorryPEVTotalWriter : NonOVCTotalWriterBase
	{
		public LorryPEVTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _cifFactory.GetElectricEnergyConsumptionLorry();

		protected override ICO2Writer CO2Writer => null;

		protected override IElectricRangeWriter ElectricRangeWriter => _cifFactory.GetElectricRangeWriter();

		#endregion
	}

	public abstract class OVCTotalWriterBase : AbstractResultGroupWriter
	{
		protected OVCTotalWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var total = entry.Weighted;
			return new XElement(Cif + "Total",
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed,
					XMLHelper.ValueAsUnit(total.AverageSpeed, "km/h", 1)),
				GetFuelConsumption(entry),
				GetElectricConsumption(entry),
				GetCO2(entry),
				_cifFactory.GetElectricRangeWriter().GetElements(total),
				new XElement(Cif + "UtilityFactor", total.UtilityFactor.ToXMLFormat(3))
			);
		}

		protected abstract XElement[] GetFuelConsumption(IOVCResultEntry entry);

		#endregion

		protected abstract XElement GetElectricConsumption(IOVCResultEntry entry);

		protected abstract XElement[] GetCO2(IOVCResultEntry entry);

	}

	public class LorryHEVOVCTotalWriter : OVCTotalWriterBase
	{
		public LorryHEVOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of OVCSummaryWriterBase

		protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
		{
			return entry.Weighted.FuelConsumption.Select(e =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry.Weighted, e.Key, e.Value)).ToArray();
		}

		protected override XElement GetElectricConsumption(IOVCResultEntry entry)
		{
			return _cifFactory.GetElectricEnergyConsumptionLorry().GetElement(entry.Weighted);
		}

		protected override XElement[] GetCO2(IOVCResultEntry entry)
		{
			return _cifFactory.GetCO2ResultLorry().GetElements(entry.Weighted);
		}

		#endregion
	}


	// ---- bus

	public class BusConvTotalWriter : NonOVCTotalWriterBase
	{
		public BusConvTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionBus();
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultBus();
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusHEVNonOVCTotalWriter : NonOVCTotalWriterBase
	{
		public BusHEVNonOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => _cifFactory.GetFuelConsumptionBus();
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultBus();
		protected override IElectricRangeWriter ElectricRangeWriter => null;

		#endregion
	}

	public class BusPEVTotalWriter : NonOVCTotalWriterBase
	{
		public BusPEVTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of NonOVCTotalWriterBase

		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;

		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _cifFactory.GetElectricEnergyConsumptionBus();
		protected override ICO2Writer CO2Writer => _cifFactory.GetCO2ResultPEVBus();
		protected override IElectricRangeWriter ElectricRangeWriter => _cifFactory.GetElectricRangeWriter();

		#endregion
	}

	public class BusOVCTotalWriter : OVCTotalWriterBase
	{
		public BusOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of OVCSummaryWriterBase

		protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
		{
			return entry.Weighted.FuelConsumption.Select(e =>
				_cifFactory.GetFuelConsumptionBus().GetElement(entry.Weighted, e.Key, e.Value)).ToArray();
		}

		protected override XElement GetElectricConsumption(IOVCResultEntry entry)
		{
			return _cifFactory.GetElectricEnergyConsumptionBus().GetElement(entry.Weighted);
		}

		protected override XElement[] GetCO2(IOVCResultEntry entry)
		{
			return _cifFactory.GetCO2ResultBus().GetElements(entry.Weighted);
		}

		#endregion
	}
}