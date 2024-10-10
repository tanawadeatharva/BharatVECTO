using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.FullReportTests.DummyRun;

public class DummyRunNinjectModule : AbstractNinjectModule
{
	#region Overrides of NinjectModule

	public override void Load()
	{
		Rebind<ISimulatorFactory>().To<DummyRunDeclarationSimulatorFactory>().Named(ExecutionMode.Declaration.ToString());

		Rebind<IModalDataFactory>().To<DummyRunModDataFactory>().InSingletonScope();

		Rebind<IModalDataContainer>().To<DummyRunModalDataContainer>();
		Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
		Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
		Rebind<IPowertrainBuilder>().To<DummyRunPowertrainBuilder>();
	}

    #endregion
}

public class DummyRunModDataFactory : IModalDataFactory
{
	#region Implementation of IModalDataFactory

	public IModalDataContainer CreateModDataContainer(VectoRunData runData, IModalDataWriter writer, Action<IModalDataContainer> addReportResult,
		IModalDataFilter[] filter)
	{
		var modData = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, runData.EngineData?.Fuels?.Select(x => x.FuelData.FuelType).ToArray() ?? null);

		var modMock = Mock.Get(modData);
		modMock.Setup(m => m.Finish(It.IsAny<VectoRun.Status>(), It.IsAny<Exception>()))
			.Callback((VectoRun.Status _, Exception e) => addReportResult(modData));

		if (runData.VehicleData.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle && runData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)) {
			//modMock.Setup(x => x.CorrectedModalData).Returns(new PEVCorrectedModalData(modData));
			var mc = Mock.Get(modData.CorrectedModalData);
			mc.Setup(x => x.FuelCorrection).Returns(new Dictionary<FuelType, IFuelConsumptionCorrection>());
		}

		return modData;
    }

	#endregion
}