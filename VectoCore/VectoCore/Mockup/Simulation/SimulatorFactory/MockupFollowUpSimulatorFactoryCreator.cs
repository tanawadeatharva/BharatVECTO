using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
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
			//IManufacturerReportFactory mrfFactory, 
			//ICustomerInformationFileFactory cifFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory,
			IXMLInputDataReader inputDataReader, bool validate) :
			base(originalStageInputData, originalReportWriter, originalDeclarationReport, simFactoryFactory,
				xmlDeclarationReportFactory, inputDataReader, validate)
		{
			//_currentStageDeclarationReport =
				//new XMLDeclarationMockupPrimaryReport(_currentStageOutputDataWriter, mrfFactory, cifFactory, vifFactory, originalStageInputData.StageInputData.ExemptedVehicle);
		}

		#region Overrides of InterimAfterPrimaryFactoryCreator

		public override ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries)
		{
			//throw new NotImplementedException();
			return base.GetNextFactory(progressEntries);
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

		public override ISimulatorFactory GetNextFactory(IDictionary<int, JobContainer.ProgressEntry> progressEntries)
		{
			return base.GetNextFactory(progressEntries);
		}

		#endregion
	}

}