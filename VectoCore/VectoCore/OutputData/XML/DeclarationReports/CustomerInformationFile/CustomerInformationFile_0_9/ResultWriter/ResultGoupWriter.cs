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
				throw new Exception("Siimulation run needs to be unsuccessful!");
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

	public class ElectricRangeWriter : AbstractResultWriter, IElectricRangeWriter
	{
		public ElectricRangeWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IElectricRangeWriter

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(Cif + "ActualChargeDepletingRange",
					XMLHelper.ValueAsUnit(result.ActualChargeDepletingRange.ConvertToKiloMeter())),
				new XElement(Cif + "EquivalentAllElectricRange",
					XMLHelper.ValueAsUnit(result.EquivalentAllElectricRange.ConvertToKiloMeter())),
				new XElement(Cif + "ZeroCO2EmissionsRange",
					XMLHelper.ValueAsUnit(result.ZeroCO2EmissionsRange.ConvertToKiloMeter())),
			};
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return new[] {
				new XElement(Cif + "ActualChargeDepletingRange",
					XMLHelper.ValueAsUnit(weightedResult.ActualChargeDepletingRange.ConvertToKiloMeter())),
				new XElement(Cif + "EquivalentAllElectricRange",
					XMLHelper.ValueAsUnit(weightedResult.EquivalentAllElectricRange.ConvertToKiloMeter())),
				new XElement(Cif + "ZeroCO2EmissionsRange",
					XMLHelper.ValueAsUnit(weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter())),
			};
		}

		#endregion
	}


}

