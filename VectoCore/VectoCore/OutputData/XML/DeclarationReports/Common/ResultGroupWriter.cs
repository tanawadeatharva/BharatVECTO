using System;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{


	public abstract class AbstractResultGroupWriter : AbstractResultWriter, IResultGroupWriter
	{
		protected AbstractResultGroupWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IResultGroupWriter

		public abstract XElement GetElement(IResultEntry entry);


		public virtual XElement GetElement(IOVCResultEntry entry)
		{
			throw new NotImplementedException();
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

	public class ResultSimulationParameterErrorWriter : AbstractResultGroupWriter
	{
		public ResultSimulationParameterErrorWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

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

	public class ElectricRangeWriter : AbstractResultWriter, IElectricRangeWriter
	{
		public ElectricRangeWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		#region Implementation of IElectricRangeWriter

		public XElement[] GetElements(IResultEntry result)
		{
			if (result.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
				};
            }
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
					result.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
					result.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
					result.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
			};
		}

		public XElement[] GetElements(IWeightedResult weightedResult)
		{
			if (weightedResult.Status == VectoRun.Status.PrimaryBusSimulationIgnore) {
				return new[] {
					new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
					new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
						new ConvertedSI(double.NaN, XMLNames.Unit_km).ValueAsUnit()),
				};
			}
            return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_ActualChargeDepletingRange,
					weightedResult.ActualChargeDepletingRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_EquivalentAllElectricRange,
					weightedResult.EquivalentAllElectricRange.ConvertToKiloMeter().ValueAsUnit()),
				new XElement(TNS + XMLNames.Report_ResultEntry_ZeroCO2EmissionsRange,
					weightedResult.ZeroCO2EmissionsRange.ConvertToKiloMeter().ValueAsUnit()),
			};
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
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Error_Val),
				ResultXMLType != null ? new XAttribute(xsi + XMLNames.XSIType, ResultXMLType) : null,
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(entry),
				_factory.GetErrorSimulationParameterWriter(_factory, TNS).GetElement(entry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(entry)
			);
		}

		public override XElement GetElement(IOVCResultEntry entry)
		{
			var errorEntry = new[] { entry.ChargeSustainingResult, entry.ChargeDepletingResult }.FirstOrDefault(x => x.Status != VectoRun.Status.Success);
			if (errorEntry == null) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(TNS + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, XMLNames.Report_Results_Status_Error_Val),
				ResultXMLType != null ? new XAttribute(xsi + XMLNames.XSIType, ResultXMLType) : null,
				_factory.GetErrorMissionWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetErrorSimulationParameterWriter(_factory, TNS).GetElement(errorEntry),
				_factory.GetErrorDetailsWriter(_factory, TNS).GetElement(errorEntry)
			);
		}

		#endregion

		protected virtual string ResultXMLType => "ResultErrorType";
	}
}