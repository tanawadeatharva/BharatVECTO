using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport
{
	public abstract class AbstractVehicleInformationFileCompleted : IXMLVehicleInformationFile
	{
		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicleInputData;
		protected IList<IManufacturingStageInputData> _manufacturingStageInputData;
		protected IManufacturingStageInputData _consolidatedInputData;
		protected IVehicleDeclarationInputData _vehicleInputData;

		#region Implementation of IXMLVehicleInformationFile

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_primaryVehicleInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.PrimaryVehicle;
			_manufacturingStageInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.ManufacturingStages;
			_consolidatedInputData = modelData.MultistageVIFInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage;
			_vehicleInputData = modelData.MultistageVIFInputData.VehicleInputData;
		}

		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			throw new System.NotImplementedException();
		}

		public void GenerateReport(XElement fullReportHash)
		{
			throw new System.NotImplementedException();
		}

		public XDocument Report { get; }
		public XNamespace Tns { get; }

		#endregion
	}


	internal abstract class VehicleInformationFile_InterimStep : AbstractVehicleInformationFileCompleted
	{
		protected VehicleInformationFile_InterimStep(IVIFReportFactory vifFactory)
		{
			
		}
	}

	internal class Conventional_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
		public Conventional_CompletedBus_VIF(IVIFReportFactory vifFactory) : base(vifFactory) {}
	}
}