using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.FullReportTests.DummyRun;

public class DummyRunModalDataContainer : ModalDataContainer
{

	public DummyRunModalDataContainer(VectoRunData runData, IModalDataWriter writer,
		Action<ModalDataContainer> addReportResult, IModalDataFilter[] filter,
		IModalDataPostProcessorFactory postProcessorFactory) : base(runData, writer, addReportResult, filter,
		postProcessorFactory)
	{
		Data.CreateColumns(ModalResults.GearboxSignals);
		Data.CreateColumns(ModalResults.DistanceCycleSignals);
		Data.CreateColumns(ModalResults.CombustionEngineSignals);
    }

	#region Overrides of ModalDataContainer

	protected override Second CalcDuration()
	{
		return 1.SI(Unit.SI.Hour).Cast<Second>();
	}

	protected override Meter CalcDistance()
	{
		return 100.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
	}

	#endregion

	
}