using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoMockup.Reports;

namespace TUGraz.VectoMockup.Simulation.SimulatorFactory
{
    internal class MockupDeclarationSimulatorFactory : SimulatorFactoryDeclaration
    {
		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer,
			IDeclarationReport declarationReport, IVTPReport vtpReport, bool validate,
			IXMLInputDataReader xmlInputDataReader, ISimulatorFactoryFactory simulatorFactoryFactory,
			IXMLDeclarationReportFactory xmlDeclarationReportFactory, IVectoRunDataFactoryFactory runDataFactoryFactory)
			: base(dataProvider, writer, declarationReport, vtpReport, validate, xmlInputDataReader,
				simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory) { }
		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider, 
			IOutputDataWriter writer, bool validate, 
			IXMLInputDataReader xmlInputDataReader, 
			ISimulatorFactoryFactory simulatorFactoryFactory, 
			IXMLDeclarationReportFactory xmlDeclarationReportFactory, 
			IVectoRunDataFactoryFactory runDataFactoryFactory) : base(dataProvider, writer, validate, xmlInputDataReader, simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory) { }

		#region Overrides of SimulatorFactory


		protected override IVectoRun GetExemptedRun(VectoRunData data)
		{

			//var addReportResult = PrepareReport(data);
			//return new MockupRun(new VehicleContainer(ExecutionMode.Declaration,
			//		new ModalDataContainer(data, ReportWriter, addReportResult))
			//	{ RunData = data });

			return base.GetExemptedRun(data);
		}

		protected override IVectoRun GetNonExemptedRun(VectoRunData data, int current, VectoRunData d, ref bool warning1Hz)
		{
			var addReportResult = PrepareReport(data);
			return new MockupRun(new VehicleContainer(ExecutionMode.Declaration,
					new ModalDataContainer(data, ReportWriter, addReportResult))
				{ RunData = data });
			
		}

		#region Overrides of SimulatorFactoryDeclaration

		protected override IFollowUpSimulatorFactoryCreator CreateFollowUpFactoryCreator(IInputDataProvider currentStageInputData,
			IDeclarationReport currentStageDeclarationReport)
		{
			switch (currentStageInputData)
			{
				case IMultistagePrimaryAndStageInputDataProvider multistagePrimaryAndStageInputDataProvider:
					return new MockupInterimAfterPrimaryFactoryCreator(
						multistagePrimaryAndStageInputDataProvider,
						ReportWriter,
						currentStageDeclarationReport,
						_simFactoryFactory, 
						((IMockupDeclarationReportFactory)_xmlDeclarationReportFactory).MrfFactory,
						((IMockupDeclarationReportFactory)_xmlDeclarationReportFactory).CifFactory, 
						_xmlInputDataReader, 
						Validate);
				case IMultistageVIFInputData multistageVifInputData:
					if (multistageVifInputData.VehicleInputData != null)
					{
						return new MockupCompletedAfterPrimaryFactoryCreator(
							multistageVifInputData,
							ReportWriter,
							currentStageDeclarationReport,
							_xmlInputDataReader,
							_simFactoryFactory, Validate);
					}
					break;
				default:
					return null;
			}
			return null;
		}

		#endregion

		#endregion
	}
}
