using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus
{
	public abstract class DeclarationDataAdapterPrimaryBus
	{
		public abstract class PrimaryBusBase : IPrimaryBusDeclarationDataAdapter
		{
			public static readonly GearboxType[] SupportedGearboxTypes =
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };
			#region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new PrimaryBusDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new PrimaryBusVehicleDataAdapter();
			protected readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			protected readonly IPTODataAdapter _ptoDataAdapter = new PTODataAdapterBus();
			protected readonly IPrimaryBusAuxiliaryDataAdapter _auxDataAdapter = new PrimaryBusAuxiliaryDataAdapter();
			protected readonly IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();

			public DriverData CreateDriverData()
			{
				return _driverDataAdapter.CreateDriverData();
			}

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
			{
				throw new NotImplementedException();
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
			}

			public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				throw new NotImplementedException();
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				throw new NotImplementedException();
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				throw new NotImplementedException();
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData);
			}

			public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				return _ptoDataAdapter.CreatePTOTransmissionData(ptoData);
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				return _auxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles);
			}

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				throw new NotImplementedException();
			}

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData vehicleData,
				VectoRunData runData)
			{
				return _auxDataAdapter.CreateBusAuxiliariesData(mission, vehicleData, runData);
			}

			#endregion
		}

		public class Conventional : PrimaryBusBase
		{
			protected IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			protected IGearboxDataAdapter _gearboxDataAdapter =
				new GearboxDataAdapter(new TorqueConverterDataAdapter());
			#region Overrides of PrimaryBusBase

			public override CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				return _gearboxDataAdapter.CreateGearshiftData(gbx, axleRatio, engineIdlingSpeed);
			}

			#endregion
		}

		public class HEV_S2 : PrimaryBusBase
		{

		}

		public class HEV_S3 : PrimaryBusBase
		{

		}

		public class HEV_S4 : PrimaryBusBase
		{

		}

		public class HEV_S_IEPC : PrimaryBusBase
		{

		}

		public class HEV_P1 : PrimaryBusBase
		{

		}

		public class HEV_P2 : PrimaryBusBase
		{

		}

		public class HEV_P2_5 : PrimaryBusBase
		{

		}

		public class HEV_P3 : PrimaryBusBase
		{

		}

		public class HEV_P4 : PrimaryBusBase
		{

		}

		public class PEV_E2 : PrimaryBusBase
		{

		}

		public class PEV_E3 : PrimaryBusBase
		{

		}


		public class PEV_E4 : PrimaryBusBase
		{

		}


		public class PEV_E_IEPC : PrimaryBusBase
		{

		}


		public class Exempted : PrimaryBusBase
		{
			#region Overrides of PrimaryBusBase

			public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			}

			#endregion
		}
    }
}
