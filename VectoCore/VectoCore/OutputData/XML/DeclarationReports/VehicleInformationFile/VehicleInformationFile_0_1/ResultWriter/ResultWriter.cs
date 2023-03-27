using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.
	ResultWriter
{

	public abstract class VIFResultWriterBase : AbstractResultGroupWriter
	{
		protected VIFResultWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				//new XAttribute(xsi + "type", ResultXMLType),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry),
				SimulationParameterWriter.GetElement(entry),
				entry.FuelData.Select(f =>
					FuelConsumptionWriter?.GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				ElectricEnergyConsumptionWriter?.GetElement(entry)
			);
		}
		#endregion

		//public virtual string ResultXMLType => "ResultPrimaryVehicleType";

		public abstract IResultGroupWriter SimulationParameterWriter { get; }

		protected abstract IFuelConsumptionWriter FuelConsumptionWriter { get; }

		protected abstract IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter { get; }

	}

	public class VIFConvResultWriter : VIFResultWriterBase
	{
		public VIFConvResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase
		
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => null;

		#endregion
	}

	public class VIFHEVNonOVCResultWriter : VIFResultWriterBase
	{
		public VIFHEVNonOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase
		
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		protected override IFuelConsumptionWriter FuelConsumptionWriter => _factory.GetFuelConsumptionBus(_factory, TNS);
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);

		#endregion
	}

	public class VIFPEVResultWriter : VIFResultWriterBase
	{
		public VIFPEVResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of CIFResultWriterBase
		
		public override IResultGroupWriter SimulationParameterWriter => _factory.GetBusSimulationParameterWriter(_factory, TNS);
		protected override IFuelConsumptionWriter FuelConsumptionWriter => null;
		protected override IElectricEnergyConsumptionWriter ElectricEnergyConsumptionWriter => _factory.GetElectricEnergyConsumptionBus(_factory, TNS);

		#endregion
	}

	public class VIFHEVOVCResultWriter : AbstractResultGroupWriter
	{

		public VIFHEVOVCResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Success_Val),
				_factory.GetSuccessMissionWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetBusSimulationParameterWriter(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetBusHEVOVCResultWriterChargeDepleting(_factory, TNS).GetElement(entry.ChargeDepletingResult),
				_factory.GetBusHEVOVCResultWriterChargeSustaining(_factory, TNS).GetElement(entry.ChargeSustainingResult)
			);
		}

		#endregion

	}

	public abstract class VIFOVCCModeWriterBase : AbstractResultGroupWriter
	{
		protected VIFOVCCModeWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_Results_OVCMode,
				new XAttribute(XMLNames.Results_Report_OVCModeAttr, OVCMode),
				entry.FuelData.Select(f =>
					_factory.GetFuelConsumptionBus(_factory, TNS).GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_factory.GetElectricEnergyConsumptionBus(_factory, TNS).GetElement(entry)
			);
		}

		public abstract  string OVCMode { get; }
	}

	public class VIFOVCChargeDepletingWriter : VIFOVCCModeWriterBase
	{
		public VIFOVCChargeDepletingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of VIFOVCCModeWriter

		public override string OVCMode => XMLNames.Results_Report_OVCModeAttr_ChargeDepleting;
		
		#endregion
	}

	public class VIFOVCChargeSustainingWriter : VIFOVCCModeWriterBase
	{
		public VIFOVCChargeSustainingWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of VIFOVCCModeWriter

		public override string OVCMode => XMLNames.Results_Report_OVCModeAttr_ChargeSustaining;

		#endregion
	}
}