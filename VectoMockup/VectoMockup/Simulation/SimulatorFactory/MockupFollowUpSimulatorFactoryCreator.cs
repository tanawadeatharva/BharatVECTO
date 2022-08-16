using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Simulation.SimulatorFactory
{
	public class MockupInterimAfterPrimaryFactoryCreator : InterimAfterPrimaryFactoryCreator
	{
		public MockupInterimAfterPrimaryFactoryCreator(
			IMultistagePrimaryAndStageInputDataProvider originalStageInputData, IOutputDataWriter originalReportWriter,
			IDeclarationReport originalDeclarationReport, 
			ISimulatorFactoryFactory simFactoryFactory,
			IManufacturerReportFactory mrfFactory, 
			ICustomerInformationFileFactory cifFactory,
			IVIFReportFactory vifFactory,
			IXMLInputDataReader inputDataReader, bool validate) :
			base(originalStageInputData, originalReportWriter, originalDeclarationReport, simFactoryFactory,
				inputDataReader, validate)
		{
			_currentStageDeclarationReport =
				new XMLDeclarationMockupPrimaryReport(_currentStageOutputDataWriter, mrfFactory, cifFactory, vifFactory, originalStageInputData.StageInputData.ExemptedVehicle);
		}

		#region Overrides of InterimAfterPrimaryFactoryCreator

		public override ISimulatorFactory GetNextFactory()
		{
			//throw new NotImplementedException();
			return base.GetNextFactory();
		}

		#endregion
	}

	public class MockupCompletedAfterPrimaryFactoryCreator : CompletedAfterInterimPrimaryFactoryCreator
	{
		public MockupCompletedAfterPrimaryFactoryCreator(IMultistageVIFInputData originalInputData,
			IOutputDataWriter originalOutputDataWriter,
			IDeclarationReport originalDeclarationReport,
			IXMLInputDataReader inputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory,
			bool validate) :
			base(originalInputData,
				originalOutputDataWriter,
				originalDeclarationReport,
				inputDataReader,
				simulatorFactoryFactory,
				validate)
		{

		}

		#region Overrides of CompletedAfterInterimPrimaryFactoryCreator

		public override ISimulatorFactory GetNextFactory()
		{
			return base.GetNextFactory();
		}

		#endregion
	}

}