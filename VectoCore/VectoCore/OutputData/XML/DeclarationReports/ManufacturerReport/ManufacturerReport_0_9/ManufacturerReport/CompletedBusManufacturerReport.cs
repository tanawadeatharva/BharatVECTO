using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport
{
	internal abstract class CompletedBusManufacturerReportBase : AbstractManufacturerReport, IXMLManufacturerReportCompletedBus
	{
		protected XNamespace _mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		private bool _allSuccess;
		public CompletedBusManufacturerReportBase(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		public override void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			var inputData = modelData.InputData as IMultistepBusInputDataProvider;
			if (inputData == null) {
				throw new VectoException("CompletedBus ManrufacturersRecordFile requires MultistepBusInputData");
			}
			Results = _resultFactory.GetMRFResultsWriter(modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(Mrf_0_9 + XMLNames.Report_InputDataSignature,
				inputData.JobInputData.ConsolidateManufacturingStage.Signature == null
					? XMLHelper.CreateDummySig(_di)
					: inputData.JobInputData.ConsolidateManufacturingStage.Signature.ToXML(_di));
		}

		#region Implementation of IXMLManufacturerReportCompletedBus

		public virtual void WriteResult(XMLDeclarationReport.ResultEntry genericResult,
			XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			_allSuccess &= genericResult.Status == VectoRun.Status.Success;
			_allSuccess &= specificResult.Status == VectoRun.Status.Success;
		}

		#endregion
	}

	internal class Conventional_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
    {
		public Conventional_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ConventionalCompletedBusManufacturerOutputDataType";



		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetConventional_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);

		}

		#endregion
	}

	internal class HEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public HEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "HEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetHEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport("HEVCompletedBusManufacturerOutputDataType");
		}

		#endregion
	}

	internal class PEV_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public PEV_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "PEVCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetPEV_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}

	internal class Exempted_CompletedBusManufacturerReport : CompletedBusManufacturerReportBase
	{
		public Exempted_CompletedBusManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory) : base(MRFReportFactory, resultFactory) { }

		#region Overrides of AbstractManufacturerReport

		public override string OutputDataType => "ExemptedCompletedBusManufacturerOutputDataType";

		protected override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _mRFReportFactory.GetExempted_CompletedBusVehicleType().GetElement(inputData);
			//GenerateReport(OutputDataType);
		}

		#endregion
	}
}
