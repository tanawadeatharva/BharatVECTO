using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class CIFResultWriterBase : AbstractResultGroupWriter
	{
		protected CIFResultWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", ResultXMLType),
				_cifFactory.GetMissionWriter().GetElement(entry),
				SimulationParameterWriter.GetElement(entry),
				ResultTotalWriter.GetElement(entry)
			);
		}
		#endregion

		public abstract string ResultXMLType { get; }

		public abstract IResultGroupWriter SimulationParameterWriter { get; }
		public abstract IResultGroupWriter ResultTotalWriter { get; }
	}

	public class LorryConvResultWriter : CIFResultWriterBase
	{

		public LorryConvResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultXMLType => "ResultSuccessConventionalType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetLorrySimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetLorryConvTotalWriter();


	}

	public class LorryHEVNonOVCResultWriter : CIFResultWriterBase
	{

		public LorryHEVNonOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultXMLType => "ResultSuccessNonOVCHEVType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetLorrySimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetLorryHEVNonOVCTotalWriter();


	}

	public class LorryPEVResultWriter : CIFResultWriterBase
	{

		public LorryPEVResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		public override string ResultXMLType => "ResultSuccessPEVType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetLorrySimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetLorryPEVTotalWriter();


	}

	public class LorryHEVOVCResultWriter : AbstractResultGroupWriter
	{

		public LorryHEVOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessOVCHEVType"),
				_cifFactory.GetMissionWriter().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetLorrySimulationParameterWriter().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetLorryHEVOVCResultWriterChargeDepleting().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetLorryHEVOVCResultWriterChargeSustaining().GetElement(entry.ChargeSustainingResult),
				_cifFactory.GetLorryHEVOVCTotalWriter().GetElement(entry)
			);
		}

		#endregion
	}

	public class LorryHEVOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public LorryHEVOVCChargeDepletingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetElectricEnergyConsumptionLorry().GetElement(entry),
				_cifFactory.GetCO2ResultLorry().GetElements(entry)
			);
		}


		#endregion
	}

	public class LorryHEVOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public LorryHEVOVCChargeSustainingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetCO2ResultLorry().GetElements(entry)
			);
		}

		#endregion
	}



	// ----- bus

	public class BusConvResultWriter : CIFResultWriterBase
	{
		public BusConvResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessConventionalType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetBusSimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetBusConvTotalWriter();

		#endregion
	}

	public class BusHEVNonOVCResultWriter : CIFResultWriterBase
	{
		public BusHEVNonOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessNonOVCHEVType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetBusSimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetBusHEVNonOVCTotalWriter();

		#endregion
	}

	public class BusPEVResultWriter : CIFResultWriterBase
	{
		public BusPEVResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of CIFResultWriterBase

		public override string ResultXMLType => "ResultSuccessPEVType";
		public override IResultGroupWriter SimulationParameterWriter => _cifFactory.GetBusSimulationParameterWriter();
		public override IResultGroupWriter ResultTotalWriter => _cifFactory.GetBusPEVTotalWriter();

		#endregion
	}
	

	public class BusHEVOVCResultWriter : AbstractResultGroupWriter
	{

		public BusHEVOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessOVCHEVType"),
				_cifFactory.GetMissionWriter().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetBusSimulationParameterWriter().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetBusHEVOVCResultWriterChargeDepleting().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetBusHEVOVCResultWriterChargeSustaining().GetElement(entry.ChargeSustainingResult),
				_cifFactory.GetBusHEVOVCTotalWriter().GetElement(entry)
			);
		}

		#endregion

	}

	public class BusOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public BusOVCChargeDepletingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionBus().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetElectricEnergyConsumptionBus().GetElement(entry),
				_cifFactory.GetCO2ResultBus().GetElements(entry)
			);
		}
		#endregion
	}

	public class BusOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public BusOVCChargeSustainingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionBus().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetCO2ResultBus().GetElements(entry)
			);
		}

		#endregion
	}
}