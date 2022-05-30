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
using TUGraz.VectoCore.Models.Simulation.Impl.Mockup;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoMockup.Simulation.SimulatorFactory
{
    internal class MockupDeclarationSimulatorFactory : SimulatorFactoryDeclaration
    {
		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer, IDeclarationReport declarationReport, IVTPReport vtpReport, bool validate, IXMLInputDataReader xmlInputDataReader, ISimulatorFactoryFactory simulatorFactoryFactory, IXMLDeclarationReportFactory xmlDeclarationReportFactory, IVectoRunDataFactoryFactory runDataFactoryFactory) : base(dataProvider, writer, declarationReport, vtpReport, validate, xmlInputDataReader, simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory) { }
		public MockupDeclarationSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer, bool validate, IXMLInputDataReader xmlInputDataReader, ISimulatorFactoryFactory simulatorFactoryFactory, IXMLDeclarationReportFactory xmlDeclarationReportFactory, IVectoRunDataFactoryFactory runDataFactoryFactory) : base(dataProvider, writer, validate, xmlInputDataReader, simulatorFactoryFactory, xmlDeclarationReportFactory, runDataFactoryFactory) { }

		#region Overrides of SimulatorFactory

		protected override IVectoRun GetExemptedRun(VectoRunData data)
		{
			throw new NotImplementedException("Exempted Mockup not implemented");
			return base.GetExemptedRun(data);
		}

		protected override IVectoRun GetNonExemptedRun(VectoRunData data, int current, VectoRunData d, ref bool warning1Hz)
		{
			var addReportResult = PrepareReport(data);
			return new MockupRun(new VehicleContainer(ExecutionMode.Declaration,
					new ModalDataContainer(data, ReportWriter, addReportResult))
				{ RunData = data });
			
		}

		#endregion
	}
}
