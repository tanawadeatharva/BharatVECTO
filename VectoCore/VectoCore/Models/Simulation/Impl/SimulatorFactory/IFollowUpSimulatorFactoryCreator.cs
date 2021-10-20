using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using JetBrains.Annotations;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public interface IFollowUpSimulatorFactoryCreator
	{ 
		ISimulatorFactory GetNextFactory();

		IOutputDataWriter CurrentStageOutputDataWriter { get; }

		IDeclarationReport CurrentStageDeclarationReport { get; }
		IInputDataProvider CurrentStageInputData { get; }
	}


	public abstract class FollowUpSimulatorFactoryCreator : IFollowUpSimulatorFactoryCreator
	{
		protected ISimulatorFactoryFactory _simulatorFactoryFactory;
		protected readonly bool _validate;

		protected FollowUpSimulatorFactoryCreator(ISimulatorFactoryFactory simFactoryFactory, bool validate)
		{
			_simulatorFactoryFactory = simFactoryFactory;
			_validate = validate;
		}

		#region Implementation of IFollowUpSimulatorFactoryCreator

		public abstract ISimulatorFactory GetNextFactory();
		public abstract IOutputDataWriter CurrentStageOutputDataWriter { get; }
		public abstract IDeclarationReport CurrentStageDeclarationReport { get; }
		public abstract IInputDataProvider CurrentStageInputData { get; }

		#endregion
	}


	public class InterimAfterPrimaryFactoryCreator : FollowUpSimulatorFactoryCreator
	{
		private readonly TempFileOutputWriter _currentStageOutputDataWriter = null;
		private readonly IOutputDataWriter _originalOriginalReportWriter = null;
		private readonly IDeclarationReport _currentStageDeclarationReport = null;
		private readonly IXMLInputDataReader _inputDataReader;
		private readonly IMultistagePrimaryAndStageInputDataProvider _originalStageInputData;
		private readonly IDeclarationReport _originalDeclarationReport;

		public InterimAfterPrimaryFactoryCreator(IMultistagePrimaryAndStageInputDataProvider originalStageInputData,
			IOutputDataWriter originalReportWriter, 
			IDeclarationReport originalDeclarationReport,
			ISimulatorFactoryFactory simFactoryFactory, 
			IXMLInputDataReader inputDataReader, bool validate) : base(simFactoryFactory, validate)
		{

			_originalStageInputData = originalStageInputData;
			_inputDataReader = inputDataReader;
			_originalOriginalReportWriter = originalReportWriter;
			_originalDeclarationReport = originalDeclarationReport;
			_currentStageOutputDataWriter =
				new TempFileOutputWriter(originalReportWriter, ReportType.DeclarationReportManufacturerXML)
				;
			_currentStageDeclarationReport =
				new XMLDeclarationReportPrimaryVehicle(_currentStageOutputDataWriter, true);

		}



		#region Implementation of IFollowUpSimulatorFactoryCreator

		public override ISimulatorFactory GetNextFactory()
		{
			//Prepare inputdata for next simulation step
			var primaryInputData = _inputDataReader.CreateDeclaration(_currentStageOutputDataWriter
				.GetDocument(ReportType.DeclarationReportPrimaryVehicleXML).CreateReader()) as IMultistageBusInputDataProvider;
			var stageInputData = _originalStageInputData.StageInputData;

			var nextStageInput = new XMLDeclarationVIFInputData(primaryInputData, stageInputData);

			var manStagesCount =
				nextStageInput.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? -1;

			_originalOriginalReportWriter.NumberOfManufacturingStages = manStagesCount;
			return _simulatorFactoryFactory.Factory(ExecutionMode.Declaration, nextStageInput, _originalOriginalReportWriter, _originalDeclarationReport,
				null, _validate);
		}

		#endregion


		#region Overrides of FollowUpSimulatorFactoryCreator

		public override IInputDataProvider CurrentStageInputData => _originalStageInputData.PrimaryVehicle;
		public override IOutputDataWriter CurrentStageOutputDataWriter => _currentStageOutputDataWriter;
		public override IDeclarationReport CurrentStageDeclarationReport => _currentStageDeclarationReport;

		#endregion

	}

	
	public class CompletedAfterInterimPrimaryFactoryCreator : FollowUpSimulatorFactoryCreator
	{
		private readonly IXMLInputDataReader _xmlInputDataReader;


		public CompletedAfterInterimPrimaryFactoryCreator(IMultistageVIFInputData originalInputData,
			IOutputDataWriter originalOutputDataWriter,
			[NotNull] IDeclarationReport originalDeclarationReport,
			IXMLInputDataReader inputDataReader,
			ISimulatorFactoryFactory simulatorFactoryFactory, 
			bool validate) : base(simulatorFactoryFactory, validate)
		{
			CurrentStageOutputDataWriter = originalOutputDataWriter;
			CurrentStageDeclarationReport = originalDeclarationReport ?? throw new ArgumentNullException(nameof(originalDeclarationReport));
			CurrentStageInputData = originalInputData;
			_xmlInputDataReader = inputDataReader;

		}
		public override ISimulatorFactory GetNextFactory()
		{
			var output = _xmlInputDataReader.CreateDeclaration(
				XmlReader.Create(CurrentStageOutputDataWriter.MultistageXmlReport.ToString().ToStream())) as IMultistageBusInputDataProvider;
			var nextStageInput = new XMLDeclarationVIFInputData(output, null);

			return _simulatorFactoryFactory.Factory(ExecutionMode.Declaration, nextStageInput, CurrentStageOutputDataWriter, CurrentStageDeclarationReport, null,
				_validate);
		}

		public override IOutputDataWriter CurrentStageOutputDataWriter { get; }
		public override IDeclarationReport CurrentStageDeclarationReport { get; }
		public override IInputDataProvider CurrentStageInputData { get; }
	}
}
