using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl {
	internal class ExemptedRun : VectoRun
	{
		private Action<ModalDataContainer> _writeSumData;

		public ExemptedRun(VehicleContainer data, Action<ModalDataContainer> writeSumData) : base(data)
		{
			_writeSumData = writeSumData;
		}

		#region Overrides of VectoRun

		public override double Progress { get { return 1; } }

		public override string CycleName { get { return "ExemptedVehicle"; } }

		public override string RunSuffix { get { return ""; } }

		protected override IResponse DoSimulationStep()
		{
			CheckValidInput();
			FinishedWithoutErrors = true;
			_writeSumData(null);
			return new ResponseCycleFinished();
		}

		private void CheckValidInput()
		{
			var vehicleData = Container.RunData.VehicleData;
			if (vehicleData.ZeroEmissionVehicle && vehicleData.DualFuelVehicle) {
				throw new VectoException("Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!");
			}

			if (!vehicleData.ZeroEmissionVehicle && !vehicleData.HybridElectricHDV && !vehicleData.DualFuelVehicle) {
				throw new VectoException("Invalid input: at least one option of ZE-HDV, He-HDV, and DualFuelVehicle has to be set for an exempted vehicle!");
			}

			if (vehicleData.HybridElectricHDV && (vehicleData.MaxNetPower1 == null || vehicleData.MaxNetPower2 == null)) {
				throw new VectoException("For He-HDV both MaxNetPower1 and MaxNetPower2 have to be provided!");
			}
		}

		protected override IResponse Initialize()
		{
			return new ResponseSuccess();
		}

		#endregion
	}
}