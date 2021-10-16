using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
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

		public FollowUpSimulatorFactoryCreator(ISimulatorFactoryFactory simFactoryFactory)
		{
			
		}

		#region Implementation of IFollowUpSimulatorFactoryCreator

		public abstract ISimulatorFactory GetNextFactory();
		public virtual IOutputDataWriter CurrentStageOutputDataWriter { get; } = null;
		public virtual IDeclarationReport CurrentStageDeclarationReport { get; } = null;

		public virtual IInputDataProvider CurrentStageInputData { get; } = null;

		#endregion
	}


	public class RunInterimAfterPrimary : FollowUpSimulatorFactoryCreator
	{
		private readonly TempFileOutputWriter _currentStageOutputDataWriter = null;
		private readonly IOutputDataWriter _originalReportWriter = null;
		private readonly IDeclarationReport _currentStageDeclarationReport = null;
		private readonly IXMLInputDataReader _inputDataReader;
		private readonly IMultistagePrimaryAndStageInputDataProvider _originalStageInputData;
		private readonly ISimulatorFactoryFactory _simFactoryFactory;
		private readonly bool _validate;

		public RunInterimAfterPrimary(IMultistagePrimaryAndStageInputDataProvider originalStageInputData,
			IOutputDataWriter reportWriter, 
			ISimulatorFactoryFactory simFactoryFactory, 
			IXMLInputDataReader inputDataReader, bool validate) : base(simFactoryFactory)
		{
			_validate = validate;
			_simFactoryFactory = simFactoryFactory;
			_originalStageInputData = originalStageInputData;
			_inputDataReader = inputDataReader;
			_originalReportWriter = reportWriter;
			_currentStageOutputDataWriter =
				new TempFileOutputWriter(reportWriter, ReportType.DeclarationReportManufacturerXML)
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

			_originalReportWriter.NumberOfManufacturingStages = manStagesCount;

			return _simFactoryFactory.Factory(ExecutionMode.Declaration, nextStageInput, _originalReportWriter, null,
				null, _validate);
		}

		#endregion


		#region Overrides of FollowUpSimulatorFactoryCreator

		public override IInputDataProvider CurrentStageInputData => _originalStageInputData.PrimaryVehicle;

		public override IOutputDataWriter CurrentStageOutputDataWriter => _currentStageOutputDataWriter;
		public override IDeclarationReport CurrentStageDeclarationReport => _currentStageDeclarationReport;

		#endregion

	}
}
