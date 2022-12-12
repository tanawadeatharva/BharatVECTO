using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.
	ResultWriter
{
	public abstract class AbstractResultWriter
	{
		protected static readonly XNamespace Cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";
		protected static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		protected ICifResultsWriterFactory _cifFactory;

		protected AbstractResultWriter(ICifResultsWriterFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}
	}



    public abstract class AbstractResultGroupWriter : AbstractResultWriter, IResultGroupWriter
	{
		protected AbstractResultGroupWriter(ICifResultsWriterFactory cifFactory): base(cifFactory) {}

		#region Implementation of IResultGroupWriter

		public abstract XElement GetElement(IResultEntry entry);

		public virtual XElement GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

        #endregion
	}

	public class ErrorResultWriter : AbstractResultGroupWriter
	{
		public ErrorResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.Success) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_cifFactory.GetMissionWriter().GetElement(entry),
				_cifFactory.GetLorrySimulationParameterWriter().GetElement(entry),
				new XElement(Cif + XMLNames.Report_Results_Error, entry.Error),
				new XElement(Cif + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
				);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var errorEntry = new[] {entry.ChargeSustainingResult, entry.ChargeDepletingResult}.FirstOrDefault(x => x.Status != VectoRun.Status.Success);
			if (errorEntry == null) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_cifFactory.GetMissionWriter().GetElement(errorEntry),
				_cifFactory.GetLorrySimulationParameterWriter().GetElement(errorEntry),
				new XElement(Cif + XMLNames.Report_Results_Error, errorEntry.Error),
				new XElement(Cif + XMLNames.Report_Results_ErrorDetails, errorEntry.StackTrace)
			);
		}

		#endregion
	}

	public class ResultMissionWriter : AbstractResultGroupWriter
	{
		public ResultMissionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat());
		}

		#endregion
	}

	public class ResultSimulationParameterLorryWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterLorryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(Cif + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(Cif + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)));
		}

		#endregion
	}

	public class LorryOVCResultWriter : AbstractResultGroupWriter
	{

		public LorryOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

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
				_cifFactory.GetLorryOVCResultWriterChargeDepleting().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetLorryOVCResultWriterChargeSustaining().GetElement(entry.ChargeSustainingResult),
				_cifFactory.GetLorryOVCSummaryWriter().GetElement(entry)
			);
		}

		#endregion
	}


	public class LorryOVCChargeDepletingWriter : AbstractResultGroupWriter
	{
		public LorryOVCChargeDepletingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetElectricEnergyConsumptionLorry().GetElement(entry),
				_cifFactory.GetCO2ResultLorry().GetElement(entry)
			);
		}


		#endregion
	}

	public class LorryOVCChargeSustainingWriter : AbstractResultGroupWriter
	{
		public LorryOVCChargeSustainingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetCO2ResultLorry().GetElement(entry)
			//GetCO2Result(entry)
			);
		}

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
				new XElement(Cif + "ActualChargeDepletingRange",
					XMLHelper.ValueAsUnit(total.ActualChargeDepletingRange.ConvertToKiloMeter())),
				new XElement(Cif + "EquivalentAllElectricRange",
					XMLHelper.ValueAsUnit(total.EquivalentAllElectricRange.ConvertToKiloMeter())),
				new XElement(Cif + "ZeroCO2EmissionsRange",
					XMLHelper.ValueAsUnit(total.ZeroCO2EmissionsRange.ConvertToKiloMeter())),
				new XElement(Cif + "UtilityFactor", total.UtilityFactor.ToXMLFormat(3))
			);
		}

		protected abstract XElement[] GetFuelConsumption(IOVCResultEntry entry);

		#endregion

		protected abstract XElement GetElectricConsumption(IOVCResultEntry entry);

		protected abstract XElement[] GetCO2(IOVCResultEntry entry);

	}

	public class LorryOVCTotalWriter : OVCTotalWriterBase
	{
		public LorryOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of OVCSummaryWriterBase

		protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
		{
			return entry.Weighted.FuelConsumption.Select(e =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry.Weighted, e.Key, e.Value)).ToArray();
		}

		protected override XElement GetElectricConsumption(IOVCResultEntry entry)
		{
			return _cifFactory.GetElectricEnergyConsumptionLorry().GetElement(entry);
		}

		protected override XElement[] GetCO2(IOVCResultEntry entry)
		{
			return _cifFactory.GetCO2ResultLorry().GetElement(entry);
		}

		#endregion
	}


	public abstract class CifSummaryWriterBase : AbstractResultWriter, ICifSummaryWriter
	{
		protected CifSummaryWriterBase(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of ICifSummaryWriter

		public XElement GetElement(IList<IResultEntry> entries)
		{
			var weighted = DeclarationData.CalculateWeightedSummary(entries);
			//return new XElement(
			//	GetSummary(weighted),
			//	weighted.FuelConsumption.Select(x => _cifFactory.GetFuelConsumptionLorry().GetElement(weighted, x.Key, x.Value)),
			//	_cifFactory.GetElectricEnergyConsumptionLorry().GetElement(weighted),
			//	_cifFactory.GetCO2ResultLorry().GetElement(weighted),
			//	);
			return null;
		}

		public XElement GetElement(IList<IOVCResultEntry> entries)
		{
			return null;
		}

		#endregion
	}

	public class LorryOVCCifSummaryWriter : CifSummaryWriterBase
	{
		public LorryOVCCifSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
	}


	// -----------------
	// bus

	public class BusOVCTotalWriter : AbstractResultGroupWriter
	{

		public BusOVCTotalWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

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
				_cifFactory.GetBusOVCResultWriterChargeDepleting().GetElement(entry.ChargeDepletingResult),
				_cifFactory.GetBusOVCResultWriterChargeSustaining().GetElement(entry.ChargeSustainingResult),
				_cifFactory.GetBusOVCSummaryWriter().GetElement(entry)
			);
		}

		#endregion
	}

	public class ResultSimulationParameterBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterBusWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(Cif + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(Cif + XMLNames.Report_Result_MassPassengers,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)),
				new XElement(Cif + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
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
				_cifFactory.GetCO2ResultBus().GetElement(entry)
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
				_cifFactory.GetCO2ResultBus().GetElement(entry)
			);
		}

		#endregion
	}

	public class BusOVCSummaryWriter : OVCTotalWriterBase
	{
		public BusOVCSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of OVCSummaryWriterBase

		protected override XElement[] GetFuelConsumption(IOVCResultEntry entry)
		{
			return entry.Weighted.FuelConsumption.Select(e =>
				_cifFactory.GetFuelConsumptionBus().GetElement(entry.Weighted, e.Key, e.Value)).ToArray();
		}

		protected override XElement GetElectricConsumption(IOVCResultEntry entry)
		{
			return _cifFactory.GetElectricEnergyConsumptionBus().GetElement(entry);
		}

		protected override XElement[] GetCO2(IOVCResultEntry entry)
		{
			return _cifFactory.GetCO2ResultBus().GetElement(entry);
		}

		#endregion
	}

	public class BusOVCCifSummaryWriter : CifSummaryWriterBase
	{
		public BusOVCCifSummaryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
	}
}

