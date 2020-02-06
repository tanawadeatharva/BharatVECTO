using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public class DeclarationDataAdapterSingleBus : IDeclarationDataAdapter {


		#region Implementation of IDeclarationDataAdapter

		public DriverData CreateDriverData()
		{
			throw new NotImplementedException();
		}

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Mission mission, Kilogram loading)
		{
			throw new NotImplementedException();
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
		{
			throw new NotImplementedException();
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
		{
			throw new NotImplementedException();
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
		{
			throw new NotImplementedException();
		}

		public CombustionEngineData CreateEngineData(
			IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode, Mission mission)
		{
			throw new NotImplementedException();
		}

		public GearboxData CreateGearboxData(
			IVehicleDeclarationInputData inputData, VectoRunData runData, IShiftPolygonCalculator shiftPolygonCalc)
		{
			throw new NotImplementedException();
		}

		public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			throw new NotImplementedException();
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarderData)
		{
			throw new NotImplementedException();
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
		{
			throw new NotImplementedException();
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData, MissionType missionType,
			VehicleClass vehicleClass, Meter vehicleLength)
		{
			throw new NotImplementedException();
		}

		public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
		{
			throw new NotImplementedException();
		}

		#endregion

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			throw new NotImplementedException();
		}
	}
}