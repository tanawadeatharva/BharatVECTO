using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
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

		//Rebind<IModalDataContainer>().To<DummyRunModalDataContainer>();
		Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
		Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
		Rebind<IPowertrainBuilder>().To<DummyRunPowertrainBuilder>().InSingletonScope();
		Rebind<IVehicleContainerFactory>().To<DummyRunVehicleContainerFactory>().InSingletonScope();
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
		var runStatus = VectoRun.Status.Pending;
		Exception ex = null;
		modMock.SetupGet(m => m.RunStatus).Returns(() => runStatus);
		modMock.SetupGet(m => m.Error).Returns(() => ex?.Message);
		modMock.SetupGet(m => m.StackTrace).Returns(() => ex == null ? null : (ex.StackTrace ?? ex.InnerException.StackTrace));
		modMock.Setup(m => m.Finish(It.IsAny<VectoRun.Status>(), It.IsAny<Exception>()))
			.Callback((VectoRun.Status s, Exception e) => {
				runStatus = s;
				ex = e;
				addReportResult(modMock.Object);
			});

		if (runData.VehicleData.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle && runData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E)) {
			//modMock.Setup(x => x.CorrectedModalData).Returns(new PEVCorrectedModalData(modData));
			var mc = Mock.Get(modData.CorrectedModalData);
			mc.Setup(x => x.FuelCorrection).Returns(new Dictionary<FuelType, IFuelConsumptionCorrection>());
		}

		return modData;
    }

	#endregion
}

public class DummyRunVehicleContainerFactory : IVehicleContainerFactory
{
	#region Implementation of IVehicleContainerFactory

	public IVehicleContainer CreateVehicleContainer(VectoRunData runData, IModalDataContainer modData, ISumData writeSumData)
	{
		var retVal = new Mock<IVehicleContainer>();
		var mc = new Mock<IMileageCounter>();
		var vi = new Mock<IVehicleInfo>();
		var gi = new Mock<IGearboxInfo>();
		retVal.Setup(c => c.MileageCounter).Returns(mc.Object);
		retVal.Setup(c => c.VehicleInfo).Returns(vi.Object);
		retVal.Setup(c => c.GearboxInfo).Returns(gi.Object);
		retVal.Setup(c => c.ModalData).Returns(modData);
		retVal.Setup(c => c.RunData).Returns(runData);
		retVal.SetupProperty(c => c.RunStatus);
		retVal.Setup(c => c.FinishSimulationRun(It.IsAny<Exception>())).Callback((Exception e) => { modData.Finish(retVal.Object.RunStatus, e);});
		mc.Setup(m => m.Distance).Returns(0.SI<Meter>());
		vi.Setup(v => v.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
		gi.Setup(g => g.Gear).Returns(new GearshiftPosition(0));
		return retVal.Object;
	}

	public ISimpleVehicleContainer CreateSimpleVehicleContainer(VectoRunData runData)
	{
		return new Mock<ISimpleVehicleContainer>().Object;
	}

	public IExemptedVehicleContainer CreateExemptedVehicleContainer(VectoRunData runData, IModalDataContainer modData,
		ISumData writeSumData)
	{
		return new Mock<IExemptedVehicleContainer>().Object;
	}

	#endregion
}