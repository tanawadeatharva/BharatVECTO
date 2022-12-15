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
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.
    ResultWriter
{
    public abstract class AbstractResultWriter
	{
		
		protected static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		protected ICommonResultsWriterFactory _factory;

		protected AbstractResultWriter(ICommonResultsWriterFactory factory, XNamespace ns)
		{
			_factory = factory;
			TNS = ns;
		}

		protected  XNamespace TNS { get; }
	}



    public abstract class AbstractResultGroupWriter : AbstractResultWriter, IResultGroupWriter
	{
		protected AbstractResultGroupWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) {}

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
		public ErrorResultWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.Success) {
				throw new Exception("Simulation run needs to be unsuccessful!");
			}

			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(entry),
				_factory.GetLorrySimulationParameterWriter(_factory, TNS).GetElement(entry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(entry)
				//new XElement(TNS + XMLNames.Report_Results_Error, entry.Error),
				//new XElement(TNS + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
				);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var errorEntry = new[] {entry.ChargeSustainingResult, entry.ChargeDepletingResult}.FirstOrDefault(x => x.Status != VectoRun.Status.Success);
			if (errorEntry == null) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetLorrySimulationParameterWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(errorEntry)
				//new XElement(TNS + XMLNames.Report_Results_Error, errorEntry.Error),
				//new XElement(TNS + XMLNames.Report_Results_ErrorDetails, errorEntry.StackTrace)
			);
		}

		#endregion
	}

	public class CIFResultMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public CIFResultMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] { new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()) };
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class ResultErrorMissionWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public ResultErrorMissionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public virtual XElement[] GetElement(IResultEntry entry)
		{
			return new[] { new XElement(TNS + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()) };
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			return null;
		}

		#endregion
	}

	public class ResultSimulationParameterLorryWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterLorryWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)));
		}

		#endregion
	}

	public class ResultSimulationParameterBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterBusWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_MassPassengers,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
			);
		}

		#endregion
	}

	public class ResultSimulationParameterMRFBusWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterMRFBusWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(TNS + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)),
				new XElement(TNS + XMLNames.Report_Result_PassengerCount,
					(entry.PassengerCount ?? double.NaN).ToXMLFormat(2))
			);
		}

		#endregion
	}

	public class VehiclePerformanceCIFWriter : AbstractResultGroupWriter
	{
		public VehiclePerformanceCIFWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Overrides of AbstractResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed,
				XMLHelper.ValueAsUnit(entry.AverageSpeed, "km/h", 1));
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			return new XElement(TNS + XMLNames.Report_ResultEntry_AverageSpeed,
				XMLHelper.ValueAsUnit(entry.Weighted.AverageSpeed, "km/h", 1));
		}

		#endregion
	}

	public class ElectricRangeWriter : AbstractResultWriter, IElectricRangeWriter
	{
		public ElectricRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IElectricRangeWriter

		public XElement[] GetElements(IResultEntry result)
		{
			return new[] {
				new XElement(TNS + "ActualChargeDepletingRange",
					XMLHelper.ValueAsUnit(result.ActualChargeDepletingRange.ConvertToKiloMeter())),
				new XElement(TNS + "EquivalentAllElectricRange",
					XMLHelper.ValueAsUnit(result.EquivalentAllElectricRange.ConvertToKiloMeter())),
				new XElement(TNS + "ZeroCO2EmissionsRange",
					XMLHelper.ValueAsUnit(result.ZeroCO2EmissionsRange.ConvertToKiloMeter())),
			};
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			return new[] {
				new XElement(TNS + "ActualChargeDepletingRange",
					XMLHelper.ValueAsUnit(weightedResult.ActualChargeDepletingRange.ConvertToKiloMeter())),
				new XElement(TNS + "EquivalentAllElectricRange",
					XMLHelper.ValueAsUnit(weightedResult.EquivalentAllElectricRange.ConvertToKiloMeter())),
				new XElement(TNS + "ZeroCO2EmissionsRange",
					XMLHelper.ValueAsUnit(weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter())),
			};
		}

		#endregion
	}

	public class CIFErrorDetailsWriter : AbstractResultWriter, IResultSequenceWriter
	{
		public CIFErrorDetailsWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultSequenceWriter

		public XElement[] GetElement(IResultEntry entry)
		{
			return new[] {
				new XElement(TNS + XMLNames.Report_Results_Error, entry.Error),
				new XElement(TNS + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
			};
		}

		public XElement[] GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

